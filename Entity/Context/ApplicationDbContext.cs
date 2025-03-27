using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;

namespace Entity.Contexts
{
    //representa el contexto de la base de datos de la aplicacion,
    //proporcionando configuraciones y metodos para la gestion de entidades y consultas personalizadas con Dapper.

    public class ApplicationDbContext : DbContext
    {

    }
}