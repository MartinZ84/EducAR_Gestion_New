BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928160829_CompletarEvaluaciones'
)
BEGIN
    DROP INDEX [IX_Evaluaciones_IdCurso] ON [Evaluaciones];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928160829_CompletarEvaluaciones'
)
BEGIN
    ALTER TABLE [Evaluaciones] ADD [Descripcion] nvarchar(4000) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928160829_CompletarEvaluaciones'
)
BEGIN
    ALTER TABLE [Evaluaciones] ADD [Temario] nvarchar(4000) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928160829_CompletarEvaluaciones'
)
BEGIN
    ALTER TABLE [Calificaciones] ADD [GeneradaPorEvaluaciones] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928160829_CompletarEvaluaciones'
)
BEGIN
    EXEC(N'ALTER TABLE [NotasEvaluacion] ADD CONSTRAINT [CK_NotasEvaluacion_Valor] CHECK ([Valor] >= 1 AND [Valor] <= 10)');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928160829_CompletarEvaluaciones'
)
BEGIN
    CREATE INDEX [IX_Evaluaciones_IdCurso_IdMateria_IdPeriodoEvaluacion_Activo] ON [Evaluaciones] ([IdCurso], [IdMateria], [IdPeriodoEvaluacion], [Activo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928160829_CompletarEvaluaciones'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260928160829_CompletarEvaluaciones', N'10.0.9');
END;

COMMIT;
GO

