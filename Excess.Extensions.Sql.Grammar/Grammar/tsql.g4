grammar tsql;

import tsql_lexer;
import primitives;
import common;
import dml;

tsql_file
    : sql_clause* EOF
    ;

sql_clause
    : dml_clause
	;