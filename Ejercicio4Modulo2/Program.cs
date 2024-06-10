using Ejercicio4Modulo2.Domain.Entities;
using Ejercicio4Modulo2.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;

namespace Ejercicio4Modulo2
{
    internal class Program
    {
        static void Main(string[] args)
        {

            #region ConexionInput

            string path = $"{AppDomain.CurrentDomain.BaseDirectory}\\data.txt";

            #endregion

            #region ConexionBBDD

            // "DefaultConnection": "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Ejercicio4Modulo2;Integrated Security=True;Trust Server Certificate=True"
            var optionBuilder = new DbContextOptionsBuilder<Ejercicio4Modulo2Context>();
            optionBuilder.UseSqlServer("Data Source = localhost\\SQLEXPRESS; Initial Catalog = Ejercicio4Modulo2; Integrated Security = True; Trust Server Certificate = True");
            var context = new Ejercicio4Modulo2Context(optionBuilder.Options);

            #endregion

            #region CargarTablas

            var feParam = context.Parametria.Where(w => w.Id == 1).FirstOrDefault();

            //Console.WriteLine($"fecha Tabla: {feParam.Value}");

            //var TParametria = context.Parametria.ToList();
            //var TRechazos = context.Rechazos.ToList();
            //var TVentasMensuales = context.VentasMensuales.ToList();

            var archivoEntrada = File.ReadAllLines(path);


            for (int i = 0; i < archivoEntrada.Length; i++)
            {

                bool registroValido = true;

                var regEntrada = archivoEntrada[i];

                var regFecha = regEntrada.Substring(0, 10);
                var regCodigo = regEntrada.Substring(10, 3);
                var regVenta = regEntrada.Substring(13, 11);
                var regVentaEG = regEntrada.Substring(24, 1);

                //Console.WriteLine($"fecha: {regFecha}");
                //Console.WriteLine($"codigo: {regCodigo}");
                //Console.WriteLine($"vta: {regVenta}");
                //Console.WriteLine($"vtaEG: {regVentaEG}");

                if (registroValido && (String.IsNullOrWhiteSpace(regFecha) || regFecha != feParam.Value.ToString()))
                {
                    registroValido = false;

                    //Console.WriteLine("REGISTRO NO VALIDO - Fecha invalida");

                    var newRechazo = new Rechazos()
                    {
                        Error = "Fecha invalida",
                        RegistroOriginal = regEntrada
                    };

                    context.Rechazos.Add(newRechazo);

                }

                if (registroValido && String.IsNullOrWhiteSpace(regCodigo))
                {
                    registroValido = false;

                    //Console.WriteLine("REGISTRO NO VALIDO - Codigo Vendedor invalido");

                    var newRechazo = new Rechazos()
                    {
                        Error = "Codigo Vendedor invalido",
                        RegistroOriginal = regEntrada
                    };

                    context.Rechazos.Add(newRechazo);

                }

                if (registroValido && String.IsNullOrWhiteSpace(regVenta))
                {

                    registroValido = false;

                    //Console.WriteLine("REGISTRO NO VALIDO - Venta invalida");

                    var newRechazo = new Rechazos()
                    {
                        Error = "Venta invalida",
                        RegistroOriginal = regEntrada
                    };

                    context.Rechazos.Add(newRechazo);

                }


                if (registroValido && (String.IsNullOrWhiteSpace(regVentaEG) || (regVentaEG != "S" && regVentaEG != "N")))
                {
                    registroValido = false;

                    //Console.WriteLine("REGISTRO NO VALIDO - Venta Empresa Grande invalida");

                    var newRechazo = new Rechazos()
                    {
                        Error = "Venta Empresa Grande invalida",
                        RegistroOriginal = regEntrada
                    };

                    context.Rechazos.Add(newRechazo);

                }


                if (registroValido)
                {

                    //Console.WriteLine("REGISTRO VALIDO");

                    var newVenta = new VentasMensuales()
                    {
                        Fecha = DateTime.Parse(regFecha),
                        CodVendedor = regCodigo,
                        Venta = decimal.Parse(regVenta),
                        VentaEmpresaGrande = (regVentaEG == "S")
                    };

                    context.VentasMensuales.Add(newVenta);

                }

                context.SaveChanges();

            }
            
            #endregion

            #region ConsultarTablas
            //5.Listar todos los vendedores que hayan superado los 100.000 en el mes.
            //Ejemplo: "El vendedor 001 vendio 250.000"
            
            var lstVendedoresOK = context.VentasMensuales.GroupBy(g => new { g.CodVendedor })
                                                        .Select(s => new 
                                                        {
                                                            CodigoVend = s.Key.CodVendedor,
                                                            TotVenta = s.Sum(v => v.Venta)
                                                        })
                                                        .Where(w => w.TotVenta > 100000)
                                                        .ToList();


            Console.WriteLine("\n\n5.Listar todos los vendedores que hayan superado los 100.000 en el mes.");
            Console.WriteLine("-----------------------------------------");
            lstVendedoresOK.ForEach(v => Console.WriteLine($"El vendedor: {v.CodigoVend} vendio {v.TotVenta}"));

            //6.Listar todos los vendedores que NO hayan superado los 100.000 en el mes.
            //Ejemplo: "El vendedor 001 vendio 90.000"

            var lstVendedoresNOK = context.VentasMensuales.GroupBy(g => new { g.CodVendedor })
                                                        .Select(s => new
                                                        {
                                                            CodigoVend = s.Key.CodVendedor,
                                                            TotVenta = s.Sum(v => v.Venta)
                                                        })
                                                        .Where(w => w.TotVenta < 90000)
                                                        .ToList();

            Console.WriteLine("\n\n6.Listar todos los vendedores que NO hayan superado los 100.000 en el mes.");
            Console.WriteLine("-----------------------------------------");
            lstVendedoresNOK.ForEach(v => Console.WriteLine($"El vendedor: {v.CodigoVend} vendio {v.TotVenta}"));


            //7.Listar todos los vendedores que haya vendido al menos una vez a una empresa grande.
            //Solo listar los codigos de vendedor

            var lstVentasEG = context.VentasMensuales.Where(w => w.VentaEmpresaGrande)
                                                        .GroupBy(g => new { g.CodVendedor })
                                                        .Select(s => new
                                                        {
                                                            CodigoVend = s.Key.CodVendedor,
                                                            CantVentasEG = s.Count()
                                                        })
                                                        .Where(h => h.CantVentasEG > 1)
                                                        .ToList();

            Console.WriteLine("\n\n7.Listar todos los vendedores que haya vendido al menos una vez a una empresa grande");
            Console.WriteLine("-----------------------------------------");
            lstVentasEG.ForEach(v => Console.WriteLine($"Vendedor: {v.CodigoVend} tuvo {v.CantVentasEG} ventas a empresas grandes"));


            //8.Listar rechazos

            var lstRechazos = context.Rechazos.ToList();

            Console.WriteLine("\n\n8.Listar rechazos");
            Console.WriteLine("-----------------------------------------");
            lstRechazos.ForEach(r => Console.WriteLine($"Error: {r.Error} ==> registro: {r.RegistroOriginal}"));

        #endregion


        Console.WriteLine("Fin");
        }

    }
}