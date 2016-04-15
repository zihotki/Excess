parser grammar dml;

import common;

// Data Manipulation Language: https://msdn.microsoft.com/en-us/library/ff848766(v=sql.120).aspx
dml_clause
	: select_statement;
	
  /*| delete_statement
	| insert_statement 
	| update_statement;*/
	

	

// DML

// https://msdn.microsoft.com/en-us/library/ms189499.aspx
select_statement
	: /*with_expression?*/ query_expression order_by_clause? /*for_clause?*/ option_clause? ';'?
	;

/*// https://msdn.microsoft.com/en-us/library/ms189835.aspx
delete_statement
	: with_expression?
	  DELETE (TOP '(' expression ')' PERCENT?)?
	  FROM? (table_alias | ddl_object | rowset_function_limited | table_var=LOCAL_ID)
	  with_table_hints?
	  output_clause?
	  (FROM table_source (',' table_source)*)?
	  (WHERE (search_condition | CURRENT OF (GLOBAL? cursor_name | cursor_var=LOCAL_ID)))?
	  for_clause? option_clause? ';'?
	;

// https://msdn.microsoft.com/en-us/library/ms174335.aspx
insert_statement
	: with_expression?
	  INSERT (TOP '(' expression ')' PERCENT?)?
	  INTO? (ddl_object | rowset_function_limited)
	  insert_with_table_hints?
	  ('(' column_name_list ')')?
	  output_clause?
	  (VALUES '(' expression_list ')' (',' '(' expression_list ')')* |
			   derived_table | execute_statement | DEFAULT VALUES)
	  for_clause? option_clause? ';'?
	;*/


/*
// https://msdn.microsoft.com/en-us/library/ms177523.aspx
update_statement
	: with_expression?
	  UPDATE (TOP '(' expression ')' PERCENT?)?
	  (ddl_object | rowset_function_limited)
	  with_table_hints?
	  SET update_elem (',' update_elem)*
	  output_clause?
	  (FROM table_source (',' table_source)*)?
	  (WHERE (search_condition_list | CURRENT OF (GLOBAL? cursor_name | cursor_var=LOCAL_ID)))?
	  for_clause? option_clause? ';'?
	;

// https://msdn.microsoft.com/en-us/library/ms177564.aspx
output_clause
	: OUTPUT output_dml_list_elem (',' output_dml_list_elem)*
			(INTO (LOCAL_ID | table_name) ('(' column_name_list ')')? )?
	;

output_dml_list_elem
	: (output_column_name | expression) (AS? column_alias)?  // TODO: scalar_expression
	;

output_column_name
	: (DELETED | INSERTED | table_name) '.' ('*' | column_name)
	| DOLLAR_ACTION
	;*/