using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducAR.API.Migrations
{
    /// <inheritdoc />
    public partial class BoletinPromedioOpcional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CalificacionFinal",
                table: "DetallesBoletines",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM DetallesBoletines WHERE CalificacionFinal IS NULL)
                    THROW 51000, 'No se puede revertir mientras existan materias sin calificación en boletines.', 1;");

            migrationBuilder.AlterColumn<decimal>(
                name: "CalificacionFinal",
                table: "DetallesBoletines",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
