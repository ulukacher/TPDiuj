﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TpIntegradorDiuj.Controllers;
using TpIntegradorDiuj.Models;

namespace TpIntegradorDiuj.Services
{
    public class BalancesService
    {
        public static List<Balance> GetAll()
        {
            TpIntegradorDbContext db = new TpIntegradorDbContext();
            return db.Balances.ToList();
        }
        public static Balance GetById(int id)
        {
            TpIntegradorDbContext db = new TpIntegradorDbContext();
            Balance bal = db.Balances.FirstOrDefault(x => x.Id == id);
            bal.Empresa = EmpresasService.GetByCUIT(bal.Empresa_CUIT);
            return bal;
        }
        public static void Editar(Balance balanceEditado)
        {
            TpIntegradorDbContext db = new TpIntegradorDbContext();
            Balance balanceAEdit = BalancesService.GetBalanceByPeriodoYEmpresa(balanceEditado.Periodo, balanceEditado.Empresa_CUIT);
            IQueryable<Cuenta> cuentasAEliminar = db.Cuentas.Where(x => x.Balance_Id == balanceEditado.Id);
            db.Cuentas.RemoveRange(cuentasAEliminar);
            db.SaveChanges();
            List<Cuenta> cuentasNuevas = balanceEditado.Cuentas;
            foreach (var item in cuentasNuevas)
            {
                item.Balance_Id = balanceAEdit.Id;
            }          
            db.Cuentas.AddRange(cuentasNuevas);

           // db.Entry(balanceEditado).State = System.Data.Entity.EntityState.Unchanged;
            db.SaveChanges();
        }
        public static bool ExisteBalanceParaEmpresaEnPeriodo(int periodo,string empresaCuit)
        {
            TpIntegradorDbContext db = new TpIntegradorDbContext();
            return db.Balances.Any(x => x.Periodo == periodo && x.Empresa_CUIT == empresaCuit);
        }
        public static Balance GetBalanceByPeriodoYEmpresa(int periodo, string empresaCuit)
        {
            TpIntegradorDbContext db = new TpIntegradorDbContext();
            return db.Balances.FirstOrDefault(x => x.Periodo == periodo && x.Empresa_CUIT == empresaCuit);
        }

        public static void CargarBalances(List<Balance> balances)
        {
            TpIntegradorDbContext db = new TpIntegradorDbContext();
            //Valido que no haya balances repetidos
            BalancesService.ValidarBalancesArchivo(balances);
            db.Balances.AddRange(balances);
            db.SaveChanges();
        }
        public static void Crear(Balance bal)
        {
            TpIntegradorDbContext db = new TpIntegradorDbContext();
            db.Balances.Add(bal);
            db.SaveChanges();
        }
        public static void Eliminar(int id)
        {
            TpIntegradorDbContext db = new TpIntegradorDbContext();
            Balance balanceAEliminar = db.Balances.FirstOrDefault(x => x.Id == id);
            db.Balances.Remove(balanceAEliminar);
            db.SaveChanges();
        }
        private static void ValidarBalancesArchivo(List<Balance> balancesArchivo)
        {
            List<Balance> balancesRepetidos = new List<Balance>();
            foreach (var item in balancesArchivo)
            {
                bool hayUnBalanceIgual = BalancesService.ExisteBalanceParaEmpresaEnPeriodo(item.Periodo, item.Empresa_CUIT);
                if (hayUnBalanceIgual)
                {
                    balancesRepetidos.Add(item);
                }

            }
            if (balancesRepetidos.Count > 0)
            {
                throw new BalancesRepetidosException("Hay balances del archivo que ya fueron cargados previamente") { Balances = balancesRepetidos };
            }
        }
        public static void BatchArchivosBalances()
        {
            //Obtener archivos en la carpeta del directorio predeterminada
            //Por cada archivo obtenido, procesarlo:
            //      Agregar los balances que no existen y modificar los balances existentes
            //      Eliminar el archivo procesado del directorio
            //Guardar cambios de la BD
        }
        public static List<int> GetPeriodosDeBalancesDeEmpresa(string cuit)
        {
            TpIntegradorDbContext db = new TpIntegradorDbContext();
            return db.Balances.Where(x => x.Empresa_CUIT == cuit).Select(x => x.Periodo).ToList();
        }
    }
}