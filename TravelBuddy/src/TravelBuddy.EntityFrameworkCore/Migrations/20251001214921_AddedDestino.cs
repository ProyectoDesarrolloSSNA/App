using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelBuddy.Migrations
{
    /// <inheritdoc />
    public partial class AddedDestino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Verificar si la tabla ya existe antes de crearla
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Destinos]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [Destinos] (
                        [Id] uniqueidentifier NOT NULL,
                        [Nombre] nvarchar(200) NOT NULL,
                        [Pais] nvarchar(100) NOT NULL,
                        [Descripcion] nvarchar(500) NOT NULL,
                        [ExtraProperties] nvarchar(max) NOT NULL,
                        [ConcurrencyStamp] nvarchar(40) NOT NULL,
                        CONSTRAINT [PK_Destinos] PRIMARY KEY ([Id])
                    )
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Destinos");
        }
    }
}
