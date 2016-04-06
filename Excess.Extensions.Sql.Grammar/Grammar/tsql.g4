grammar tsql;

import dml;

tsql_file
    : sql_clause* EOF
    ;

sql_clause
    : dml_clause
	;