import tsql_lexer;
import primitives;


another_statement
    : declare_statement
    | cursor_statement
    | execute_statement
    | security_statement
    | set_statment
    | transaction_statement
    | go_statement
    | use_statement
    ;

	

// Other statements.

// https://msdn.microsoft.com/en-us/library/ms188927.aspx
declare_statement
    : DECLARE declare_local (',' declare_local)* ';'?
    | DECLARE LOCAL_ID AS? table_type_definition ';'?
    ;

// https://msdn.microsoft.com/en-us/library/ms181441(v=sql.120).aspx
cursor_statement
    // https://msdn.microsoft.com/en-us/library/ms175035(v=sql.120).aspx
    : CLOSE GLOBAL? cursor_name ';'?
    // https://msdn.microsoft.com/en-us/library/ms188782(v=sql.120).aspx
    | DEALLOCATE GLOBAL? cursor_name ';'?
    // https://msdn.microsoft.com/en-us/library/ms180169(v=sql.120).aspx
    | declare_cursor
    // https://msdn.microsoft.com/en-us/library/ms180152(v=sql.120).aspx
    | fetch_cursor
    // https://msdn.microsoft.com/en-us/library/ms190500(v=sql.120).aspx
    | OPEN GLOBAL? cursor_name ';'?
    ;

// https://msdn.microsoft.com/en-us/library/ms188332.aspx
execute_statement
    : (EXEC | EXECUTE) (return_status=LOCAL_ID '=')? func_proc_name (execute_statement_arg (',' execute_statement_arg)*)? ';'?
    | (EXEC | EXECUTE) '(' execute_var_string ('+' execute_var_string)* ')' (AS? (LOGIN | USER) '=' STRING)? ';'?
    ;

execute_statement_arg
    : (parameter=LOCAL_ID '=')? (constant | LOCAL_ID (OUTPUT | OUT)? | DEFAULT | NULL)
    ;

execute_var_string
    : LOCAL_ID
    | STRING
    ;

// https://msdn.microsoft.com/en-us/library/ff848791.aspx
security_statement
    // https://msdn.microsoft.com/en-us/library/ms188354.aspx
    : execute_clause ';'?
    // https://msdn.microsoft.com/en-us/library/ms178632.aspx
    | REVERT ('(' WITH COOKIE '=' LOCAL_ID ')')? ';'?
    ;

// https://msdn.microsoft.com/en-us/library/ms190356.aspx
// https://msdn.microsoft.com/en-us/library/ms189484.aspx
set_statment
    : SET LOCAL_ID ('.' member_name=id)? '=' expression ';'?
    | SET LOCAL_ID assignment_operator expression ';'?
    | SET LOCAL_ID '='
      CURSOR declare_set_cursor_common (FOR (READ ONLY | UPDATE (OF column_name_list)?))? ';'?
    // https://msdn.microsoft.com/en-us/library/ms189837.aspx
    | set_special
    ;

// https://msdn.microsoft.com/en-us/library/ms174377.aspx
transaction_statement
    // https://msdn.microsoft.com/en-us/library/ms188386.aspx
    : BEGIN DISTRIBUTED (TRAN | TRANSACTION) (id | LOCAL_ID)? ';'?
    // https://msdn.microsoft.com/en-us/library/ms188929.aspx
    | BEGIN (TRAN | TRANSACTION) ((id | LOCAL_ID) (WITH MARK STRING)?)? ';'?
    // https://msdn.microsoft.com/en-us/library/ms190295.aspx
    | COMMIT (TRAN | TRANSACTION) ((id | LOCAL_ID) (WITH '(' DELAYED_DURABILITY = (OFF | ON) ')')?)? ';'?
    // https://msdn.microsoft.com/en-us/library/ms178628.aspx
    | COMMIT WORK? ';'?
    // https://msdn.microsoft.com/en-us/library/ms181299.aspx
    | ROLLBACK (TRAN | TRANSACTION) (id | LOCAL_ID)? ';'?
    // https://msdn.microsoft.com/en-us/library/ms174973.aspx
    | ROLLBACK WORK? ';'?
    // https://msdn.microsoft.com/en-us/library/ms188378.aspx
    | SAVE (TRAN | TRANSACTION) (id | LOCAL_ID)? ';'?
    ;

	
// https://msdn.microsoft.com/en-us/library/ms188037.aspx
go_statement
    : GO (count=DECIMAL)?
    ;

// https://msdn.microsoft.com/en-us/library/ms188366.aspx
use_statement
    : USE database=id ';'?
    ;*/