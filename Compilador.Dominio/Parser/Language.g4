grammar Language;

program: line* EOF;

line: statement | ifBlock | whileBlock;

statement: (assignment | functionCall) ';';

ifBlock: 'IF' expression block ('ELSE' elseIfBlock)?;

elseIfBlock: block | ifBlock;

whileBlock: 'WHILE' expression block;

assignment: ID '=' expression;

functionCall: ID '(' expression (expression (',' expression)*)? ')';

expression
    : constant                          #constantExpression
    | ID                                #idExpression
    | functionCall                      #functionCallExpression
    | '(' expression ')'                #parenthesizedExpression
    | '!' expression                    #notExpression
    | expression multOp expression      #multiplicativeExpression
    | expression addOp expression       #additiveExpression
    | expression compareOp expression   #comparisonExpression
    | expression boolOp expression      #booleanExpression
    ;

multOp: '*' | '/' | '%';
addOp: '+' | '-';
compareOp: 'COMP';
boolOp: BOOL_OP;

BOOL_OP: 'and' | 'or';

constant: INTEGER | FLOAT | STRING | BOOL | NULL;

INTEGER: 'NUM_INT';
FLOAT: 'NUM_DEC';
STRING: 'Texto';
BOOL: 'true' | 'false';
NULL: 'null';

block: '{' line* '}';

WS: [ \t\r\n]+ -> skip;
ID: 'ID';
