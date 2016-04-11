import tsql_lexer;
import primitives;

// Control-of-Flow Language: https://msdn.microsoft.com/en-us/library/ms174290.aspx
// Labels for better AST traverse.
cfl_statement
    // https://msdn.microsoft.com/en-us/library/ms190487.aspx
    : BEGIN ';'? sql_clause* END ';'?                #begin_statement
    // https://msdn.microsoft.com/en-us/library/ms181271.aspx
    | BREAK ';'?                                     #break_statement
    // https://msdn.microsoft.com/en-us/library/ms174366.aspx
    | CONTINUE ';'?                                  #continue_statement
    // https://msdn.microsoft.com/en-us/library/ms180188.aspx
    | GOTO id ';'?                                   #goto_statement
    | id ':' ';'?                                    #goto_statement
    // https://msdn.microsoft.com/en-us/library/ms182717.aspx
    | IF search_condition sql_clause (ELSE sql_clause)? ';'?  #if_statement
    // https://msdn.microsoft.com/en-us/library/ms174998.aspx
    | RETURN expression? ';'?                        #return_statement
    // https://msdn.microsoft.com/en-us/library/ee677615.aspx
    | THROW (
      (DECIMAL | LOCAL_ID) ',' (STRING | LOCAL_ID) ',' (DECIMAL | LOCAL_ID))? ';'?  #throw_statement
    // https://msdn.microsoft.com/en-us/library/ms175976.aspx
    | BEGIN TRY ';'? sql_clause* END TRY ';'?
      BEGIN CATCH ';'? sql_clause* END CATCH ';'?                                   #try_catch_statement
    // https://msdn.microsoft.com/en-us/library/ms187331.aspx
    | WAITFOR (DELAY | TIME)  expression ';'?                                       #waitfor_statement
    // https://msdn.microsoft.com/en-us/library/ms178642.aspx
    | WHILE search_condition (sql_clause | BREAK ';'? | CONTINUE ';'?)              #while_statement
    // https://msdn.microsoft.com/en-us/library/ms176047.aspx.
    | PRINT expression ';'?                                                         #print_statement
    // https://msdn.microsoft.com/en-us/library/ms178592.aspx
    | RAISERROR '(' msg=(DECIMAL | STRING | LOCAL_ID) ',' (number | LOCAL_ID) ','
       (number | LOCAL_ID) (',' (constant | LOCAL_ID))* ')' ';'?                    #raiseerror_statement
    ;