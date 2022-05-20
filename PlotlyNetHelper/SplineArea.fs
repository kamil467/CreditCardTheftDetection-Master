namespace PlotlyNetHelper
open Plotly.NET
module SplineArea =

  let area2  (x:int[], y:int[])  =
    [Chart.SplineArea(y,x)] |> Chart.combine

