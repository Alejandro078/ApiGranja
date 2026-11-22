# API de gestión de una granja lechera

Solución del taller práctico de Lenguajes de Programación. Se conserva la solución inicial y se completa con las operaciones solicitadas en el enunciado.

## Requisitos

- Visual Studio 2022 con soporte para .NET 10, o SDK de .NET 10.
- Restauración de paquetes NuGet.

## Abrir y ejecutar

1. Extraer el ZIP.
2. Abrir `Web/ApiSampleFinal.sln`.
3. Establecer `ApiSampleFinal` como proyecto de inicio.
4. Ejecutar con F5. En Development, Swagger queda disponible en `http://localhost:57200/swagger` (el puerto depende del perfil de ejecución).

La aplicación aplica la migración inicial de SQLite al iniciar y prepara la configuración de la granja. La primera configuración utiliza los valores sugeridos en el taller: 10 vacas, 15 litros por vaca al día, rendimiento de queso de 10 litros/kg, rendimiento de yogurt de 1 litro/L, vida útil de 30 y 15 días. El precio de queso inicia en $28.000, tomado del ejemplo de venta del enunciado. El precio de yogurt inicia en cero porque el taller no fija un valor; se debe configurar antes de registrar ventas de yogurt.

## Proyectos

- `Domain`: entidades, enum, excepciones e interfaces del dominio.
- `Infrastructure`: SQLite, Entity Framework Core, migración inicial, repositorios, reloj y Unit of Work.
- `Services`: DTOs y servicios de configuración, producción, inventario, ventas y reportes.
- `Web`: controladores, Swagger, AutoMapper, inyección de dependencias y middleware `ProblemDetails`.
- `Tests/Granja.Tests`: pruebas unitarias de las reglas principales.

## Endpoints

| Método | Ruta | Función |
|---|---|---|
| GET | `/api/granja` | Consultar configuración |
| PUT | `/api/granja` | Actualizar número de vacas, litros, rendimientos, vida útil y precios |
| POST | `/api/produccion` | Registrar producción diaria por vaca |
| POST | `/api/produccion/transformar` | Transformar litros de una fecha en queso o yogurt |
| GET | `/api/inventario` | Consultar lotes, existencias y producto dañado |
| POST | `/api/inventario/procesar-vencimientos?fecha=yyyy-MM-dd` | Procesar lotes vencidos |
| POST | `/api/ventas` | Registrar venta aplicando FIFO |
| GET | `/api/ventas?desde=&hasta=` | Consultar ventas |
| GET | `/api/reportes/ventas?desde=&hasta=&tipo=` | Reporte de ventas y total general |
| GET | `/api/reportes/leche-diaria?desde=&hasta=` | Leche producida, transformada y no aprovechada por día |
| GET | `/api/reportes/producto-diario?desde=&hasta=` | Producto elaborado por día y tipo |
| GET | `/api/reportes/producto-total` | Totales producidos, vendidos, disponibles y dañados |
| GET | `/api/reportes/producto-danado?desde=&hasta=` | Producto dañado por lote |
| GET | `/api/reportes/vacas?desde=&hasta=` | Promedio, mayor y menor producción por vaca, y vacas bajo el promedio |

Las fechas aceptan el formato `yyyy-MM-dd`. En las operaciones se puede omitir la fecha para usar la fecha del reloj del sistema.

## Ejemplos de solicitudes

Consultar la configuración:

```http
GET /api/granja
```

Actualizar la configuración:

```json
{
  "numeroVacas": 10,
  "litrosPorVacaDia": 15,
  "rendimientoQueso": 10,
  "rendimientoYogurt": 1,
  "vidaUtilQueso": 30,
  "vidaUtilYogurt": 15,
  "precioQueso": 28000,
  "precioYogurt": 1000
}
```

Registrar producción:

```json
{ "fecha": "2026-12-01" }
```

Transformar 100 litros en queso:

```json
{ "fecha": "2026-12-01", "tipoProducto": "Queso", "litros": 100 }
```

Registrar venta:

```json
{ "tipoProducto": "Queso", "cantidad": 12, "fecha": "2026-12-03" }
```

## Reglas implementadas

- La producción diaria se registra por vaca y fecha, con restricción para evitar duplicados.
- La transformación consume solo leche disponible de la fecha seleccionada.
- Cada transformación genera un lote con fecha de vencimiento y cantidades inicial, disponible y dañada.
- La venta utiliza primero el lote más antiguo que no esté vencido. Se valida toda la cantidad antes de modificar el inventario y se persiste la venta con sus detalles en una única llamada a `SaveChangesAsync`.
- Los lotes vencidos pasan su cantidad disponible a dañada y dejan de estar disponibles para la venta.
- Las operaciones aceptan fecha explícita o utilizan `IClock`.
- Los errores se devuelven mediante `ProblemDetails` con códigos 400, 404 y 409.

## Pruebas

Desde la carpeta que contiene la solución:

```bash
dotnet test Web/ApiSampleFinal.sln
```

Las pruebas cubren producción diaria y duplicados, transformación por encima de la leche disponible, ventas FIFO entre varios lotes, rechazo de lotes vencidos, atomicidad ante existencias insuficientes y procesamiento/reporte de producto dañado.
