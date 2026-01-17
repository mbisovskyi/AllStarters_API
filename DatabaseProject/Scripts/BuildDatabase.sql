PRINT('--- Executing Schema Scripts ---')
:r .\CreateTable\ValidValues\ValidUserRoles_CreateTable.sql

PRINT('--- Executing Insert Data Scripts ---')
:r .\InsertTable\ValidValues\ValidUserRoles_InsertTable.sql