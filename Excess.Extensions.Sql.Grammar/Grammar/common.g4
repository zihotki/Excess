parser grammar common;

import primitives;


/*execute_clause
    : (EXEC | EXECUTE) AS clause=(CALLER | SELF | OWNER | STRING)
    ;

declare_local
    : LOCAL_ID AS? data_type ('=' expression)?
    ;*/



/*// https://msdn.microsoft.com/en-us/library/ms180169.aspx
declare_cursor
    : DECLARE cursor_name CURSOR ';'?
    | DECLARE cursor_name INSENSITIVE? SCROLL? CURSOR FOR select_statement
      (FOR (READ ONLY | UPDATE | (OF column_name_list)))? ';'?
    | DECLARE cursor_name
      CURSOR  declare_set_cursor_common (FOR UPDATE (OF column_name_list)?)? ';'?
    ;

declare_set_cursor_common
    : (LOCAL | GLOBAL)?
      (FORWARD_ONLY | SCROLL)? (STATIC | KEYSET | DYNAMIC | FAST_FORWARD)?
      (READ_ONLY | SCROLL_LOCKS | OPTIMISTIC)? TYPE_WARNING?
      FOR select_statement
    ;

fetch_cursor
    : FETCH ((NEXT | PRIOR | FIRST | LAST | ABSOLUTE expression | RELATIVE expression)? FROM)?
      GLOBAL? cursor_name (INTO LOCAL_ID (',' LOCAL_ID)*)? ';'?
    ;
	*/
/*// https://msdn.microsoft.com/en-us/library/ms190356.aspx
// Runtime check.
set_special
    : SET id (id | constant | LOCAL_ID | on_off) ';'?
	;
    // https://msdn.microsoft.com/en-us/library/ms173763.aspx
    | SET TRANSACTION ISOLATION LEVEL
      (READ UNCOMMITTED | READ COMMITTED | REPEATABLE READ | SNAPSHOT | SERIALIZABLE) ';'?
    // https://msdn.microsoft.com/en-us/library/ms188059.aspx
    | SET IDENTITY_INSERT table_name on_off ';'?
    ;*/

// Expression.

// https://msdn.microsoft.com/en-us/library/ms190286.aspx
// Operator precendence: https://msdn.microsoft.com/en-us/library/ms190276.aspx
expression
    : DEFAULT                                                  #primitive_expression
    | NULL                                                     #primitive_expression
    | LOCAL_ID                                                 #primitive_expression
    | constant                                                 #primitive_expression
    | function_call                                            #function_call_expression
    | expression COLLATE id                                    #function_call_expression
    | case_expr                                                #case_expression
    | full_column_name                                         #column_ref_expression
    | '(' expression ')'                                       #bracket_expression
    | '(' subquery ')'                                         #subquery_expression
    | '~' expression                                           #unary_operator_expression

    | expression op=('*' | '/' | '%') expression               #binary_operator_expression
    | op=('+' | '-') expression                                #unary_operator_expression
    | expression op=('+' | '-' | '&' | '^' | '|') expression   #binary_operator_expression
    | expression comparison_operator expression                #binary_operator_expression
    ;

constant_expression
    : NULL
    | constant
    // system functions: https://msdn.microsoft.com/en-us/library/ms187786.aspx
    | function_call
    | LOCAL_ID         // TODO: remove.
    | '(' constant_expression ')'
    ;

subquery
    : select_statement
    ;

/*// https://msdn.microsoft.com/en-us/library/ms175972.aspx
with_expression
    : WITH (XMLNAMESPACES ',')? common_table_expression (',' common_table_expression)*
    ;

common_table_expression
    : expression_name=id ('(' column_name_list ')')? AS '(' select_statement ')'
    ;

update_elem
    : (full_column_name | LOCAL_ID) ('=' | assignment_operator) expression
    | udt_column_name=id '.' method_name=id '(' expression_list ')'
    //| full_column_name '.' WRITE (expression, )
    ;
	*/

// https://msdn.microsoft.com/en-us/library/ms173545.aspx
search_condition_list
    : search_condition (',' search_condition)*
    ;

search_condition
    : search_condition_or (AND search_condition_or)*
    ;

search_condition_or
    : search_condition_not (OR search_condition_not)*
    ;

search_condition_not
    : NOT? predicate
    ;

predicate
    : EXISTS '(' subquery ')'
    | expression comparison_operator expression
    | expression comparison_operator (ALL | SOME | ANY) '(' subquery ')'
    | expression NOT? BETWEEN expression AND expression
    | expression NOT? IN '(' (subquery | expression_list) ')'
    | expression NOT? LIKE expression (ESCAPE expression)?
    | expression IS null_notnull
    | '(' search_condition ')'
    ;

query_expression
    : (query_specification | '(' query_expression ')') union*
    ;

union
    : (UNION ALL? | EXCEPT | INTERSECT) (query_specification | ('(' query_expression ')')+)
    ;

// https://msdn.microsoft.com/en-us/library/ms176104.aspx
query_specification
    : SELECT (ALL | DISTINCT)? (TOP expression PERCENT? (WITH TIES)?)?
      select_list
      // https://msdn.microsoft.com/en-us/library/ms188029.aspx
      /*(INTO into_table=table_name)? //select into is not supported yet*/
      (FROM table_source (',' table_source)*)?
      (WHERE where=search_condition)?
      // https://msdn.microsoft.com/en-us/library/ms177673.aspx
      (GROUP BY group_by_item (',' group_by_item)*)?
      (HAVING having=search_condition)?
    ;

// https://msdn.microsoft.com/en-us/library/ms188385.aspx
order_by_clause
    : ORDER BY order_by_expression (',' order_by_expression)*
      (OFFSET expression (ROW | ROWS) (FETCH (FIRST | NEXT) expression (ROW | ROWS) ONLY)?)?
    ;
/*
// https://msdn.microsoft.com/en-us/library/ms173812.aspx
for_clause
    : FOR BROWSE
    | FOR XML AUTO xml_common_directives?
    | FOR XML PATH ('(' STRING ')')? xml_common_directives?
    ;

xml_common_directives
    : ',' (BINARY BASE64 | TYPE | ROOT)
    ;
	*/

order_by_expression
    : expression (ASC | DESC)?
    ;

group_by_item
    : expression
    /*| rollup_spec
    | cube_spec
    | grouping_sets_spec
    | grand_total*/
    ;

option_clause
    // https://msdn.microsoft.com/en-us/library/ms181714.aspx
    : OPTION '(' option (',' option)* ')'
    ;

option
    : FAST number_rows=DECIMAL
    | (HASH | ORDER) GROUP
    | (MERGE | HASH | CONCAT) UNION
    | KEEPFIXED PLAN
    | OPTIMIZE FOR '(' optimize_for_arg (',' optimize_for_arg)* ')'
    | OPTIMIZE FOR UNKNOWN
    ;

optimize_for_arg
    : LOCAL_ID (UNKNOWN | '=' constant)
    ;

// https://msdn.microsoft.com/en-us/library/ms176104.aspx
select_list
    : select_list_elem (',' select_list_elem)*
    ;

select_list_elem
    : (table_name '.')? ('*' | '$' (IDENTITY | ROWGUID))
    | column_alias '=' expression
    | expression (AS? column_alias)?
    ;

partition_by_clause
    : PARTITION BY expression_list
    ;

// https://msdn.microsoft.com/en-us/library/ms177634.aspx
table_source
    : table_source_item_joined
    | '(' table_source_item_joined ')'
    ;

table_source_item_joined
    : table_source_item join_part*
    ;

table_source_item
    : table_name_with_hint        as_table_alias?
    | rowset_function             as_table_alias?
    | derived_table              (as_table_alias column_alias_list?)?
    /*| change_table                as_table_alias*/
    | function_call               as_table_alias?
    | LOCAL_ID                    as_table_alias?
    | LOCAL_ID '.' function_call (as_table_alias column_alias_list?)?
    ;

/*change_table
	: CHANGETABLE '(' CHANGES table_name ',' (NULL | DECIMAL | LOCAL_ID) ')'
	;
*/

// https://msdn.microsoft.com/en-us/library/ms191472.aspx
join_part
    // https://msdn.microsoft.com/en-us/library/ms173815(v=sql.120).aspx
    : (INNER? |
       join_type=(LEFT | RIGHT | FULL) OUTER?) (join_hint=(LOOP | HASH | MERGE | REMOTE))?
       JOIN table_source ON search_condition
    | CROSS JOIN table_source
    | CROSS APPLY table_source
    | OUTER APPLY table_source
    ;

table_name_with_hint
    : table_name with_table_hints?
    ;

// https://msdn.microsoft.com/en-us/library/ms190312.aspx
rowset_function
    : OPENROWSET '(' BULK data_file=STRING ',' (bulk_option (',' bulk_option)* | id)')'
    ;

// runtime check.
bulk_option
    : id '=' (DECIMAL | STRING)
    ;

derived_table
    : subquery
    | '(' subquery ')'
    ;

function_call
    : ranking_windowed_function
    | aggregate_windowed_function
    | scalar_function_name '(' expression_list? ')'
    // https://msdn.microsoft.com/en-us/library/ms173784.aspx
    | BINARY_CHECKSUM '(' '*' ')'
    // https://msdn.microsoft.com/en-us/library/hh231076.aspx
    // https://msdn.microsoft.com/en-us/library/ms187928.aspx
    | CAST '(' expression AS data_type ')'
    | CONVERT '(' data_type ',' expression (',' style=expression)? ')'
    // https://msdn.microsoft.com/en-us/library/ms189788.aspx
    | CHECKSUM '(' '*' ')'
    // https://msdn.microsoft.com/en-us/library/ms190349.aspx
    | COALESCE '(' expression_list ')'
    // https://msdn.microsoft.com/en-us/library/ms188751.aspx
    | CURRENT_TIMESTAMP
    // https://msdn.microsoft.com/en-us/library/ms176050.aspx
    | CURRENT_USER
    // https://msdn.microsoft.com/en-us/library/ms186819.aspx
    | DATEADD '(' datepart ',' expression ',' expression ')'
    // https://msdn.microsoft.com/en-us/library/ms189794.aspx
    | DATEDIFF '(' datepart ',' expression ',' expression ')'
    // https://msdn.microsoft.com/en-us/library/ms174395.aspx
    | DATENAME '(' datepart ',' expression ')'
    // https://msdn.microsoft.com/en-us/library/ms174420.aspx
    | DATEPART '(' datepart ',' expression ')'
    // https://msdn.microsoft.com/en-us/library/ms189838.aspx
    | IDENTITY '(' data_type (',' seed=DECIMAL)? (',' increment=DECIMAL)? ')'
    // https://msdn.microsoft.com/en-us/library/bb839514.aspx
    | MIN_ACTIVE_ROWVERSION
    // https://msdn.microsoft.com/en-us/library/ms177562.aspx
    | NULLIF '(' expression ',' expression ')'
    // https://msdn.microsoft.com/en-us/library/ms177587.aspx
    | SESSION_USER
    // https://msdn.microsoft.com/en-us/library/ms179930.aspx
    | SYSTEM_USER
    ;

datepart
    : ID
    ;

as_table_alias
    : AS? table_alias
    ;

table_alias
    : id with_table_hints?
    ;

// https://msdn.microsoft.com/en-us/library/ms187373.aspx
with_table_hints
    : WITH? '(' table_hint (',' table_hint)* ')'
    ;

// https://msdn.microsoft.com/en-us/library/ms187373.aspx
insert_with_table_hints
    : WITH '(' table_hint (',' table_hint)* ')'
    ;

// Id runtime check. Id can be (FORCESCAN, HOLDLOCK, NOLOCK, NOWAIT, PAGLOCK, READCOMMITTED,
// READCOMMITTEDLOCK, READPAST, READUNCOMMITTED, REPEATABLEREAD, ROWLOCK, TABLOCK, TABLOCKX
// UPDLOCK, XLOCK)

table_hint
    : NOEXPAND? ( INDEX '(' index_value (',' index_value)* ')'
                | INDEX '=' index_value
                | FORCESEEK ('(' index_value '(' index_column_name  (',' index_column_name)* ')' ')')?
                | SERIALIZABLE
                | SNAPSHOT
                | SPATIAL_WINDOW_MAX_CELLS '=' DECIMAL
                | ID)?
    ;

index_column_name
	: ID
	;

index_value
    : ID | DECIMAL
    ;

column_alias_list
    : '(' column_alias (',' column_alias)* ')'
    ;

column_alias
    : id
    | STRING
    ;
    

expression_list
    : expression (',' expression)*
    ;

// https://msdn.microsoft.com/en-us/library/ms181765.aspx
case_expr
    : CASE expression (WHEN expression THEN expression)+ (ELSE expression)? END
    | CASE (WHEN search_condition THEN expression)+ (ELSE expression)? END
    ;

// https://msdn.microsoft.com/en-us/library/ms189798.aspx
ranking_windowed_function
    : RANK '(' ')' over_clause
    | DENSE_RANK '(' ')' over_clause
    | NTILE '(' expression ')' over_clause
    | ROW_NUMBER '(' ')' over_clause
    ;

// https://msdn.microsoft.com/en-us/library/ms173454.aspx
aggregate_windowed_function
    : AVG '(' all_distinct_expression ')' over_clause?
    | CHECKSUM_AGG '(' all_distinct_expression ')'
    | GROUPING '(' expression ')'
    | GROUPING_ID '(' expression_list ')'
    | MAX '(' all_distinct_expression ')' over_clause?
    | MIN '(' all_distinct_expression ')' over_clause?
    | SUM '(' all_distinct_expression ')' over_clause?
    | STDEV '(' all_distinct_expression ')' over_clause?
    | STDEVP '(' all_distinct_expression ')' over_clause?
    | VAR '(' all_distinct_expression ')' over_clause?
    | VARP '(' all_distinct_expression ')' over_clause?
    | COUNT '(' ('*' | all_distinct_expression) ')' over_clause?
    | COUNT_BIG '(' ('*' | all_distinct_expression) ')' over_clause?
    ;

all_distinct_expression
    : (ALL | DISTINCT)? expression
    ;

// https://msdn.microsoft.com/en-us/library/ms189461.aspx
over_clause
    : OVER '(' partition_by_clause? order_by_clause? row_or_range_clause? ')'
    ;

row_or_range_clause
    : (ROWS | RANGE) window_frame_extent
    ;

window_frame_extent
    : window_frame_preceding
    | BETWEEN window_frame_bound AND window_frame_bound
    ;

window_frame_bound
    : window_frame_preceding
    | window_frame_following
    ;

window_frame_preceding
    : UNBOUNDED PRECEDING
    | DECIMAL PRECEDING
    | CURRENT ROW
    ;

window_frame_following
    : UNBOUNDED FOLLOWING
    | DECIMAL FOLLOWING
    ;


