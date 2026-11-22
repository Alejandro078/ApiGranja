using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class LoteProducto
    {
        public int Id { get; private set; }

        public TipoProducto Tipo { get; private set; }

        public DateOnly FechaProduccion { get; private set; }

        public DateOnly FechaVencimiento { get; private set; }

        public decimal CantidadInicial { get; private set; }

        public decimal CantidadDisponible { get; private set; }

        public decimal CantidadDanada { get; private set; }

        // Constructor para Entity Framework
        private LoteProducto() { }

        public LoteProducto(
            TipoProducto tipo,
            DateOnly fechaProduccion,
            decimal cantidad,
            int vidaUtil)
        {
            if (cantidad <= 0)
                throw new ArgumentException(
                    "La cantidad debe ser mayor a cero.");

            if (vidaUtil <= 0)
                throw new ArgumentException(
                    "La vida útil debe ser mayor a cero.");

            Tipo = tipo;
            FechaProduccion = fechaProduccion;
            FechaVencimiento = fechaProduccion.AddDays(vidaUtil);
            CantidadInicial = cantidad;
            CantidadDisponible = cantidad;
            CantidadDanada = 0;
        }

        public bool EstaVencido(DateOnly fecha)
        {
            return fecha > FechaVencimiento;
        }

        public void Descontar(decimal cantidad)
        {
            if (cantidad <= 0 || cantidad > CantidadDisponible)
                throw new InvalidOperationException(
                    "Cantidad inválida para el lote.");

            CantidadDisponible -= cantidad;
        }

        public decimal MarcarComoDanado()
        {
            decimal perdida = CantidadDisponible;

            CantidadDanada += perdida;
            CantidadDisponible = 0;

            return perdida;
        }
    }
}
