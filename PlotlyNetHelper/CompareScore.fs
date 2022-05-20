namespace PlotlyNetHelper
open Plotly.NET
module CompareScore = 
let labels =["Logistic Regerssion";"Logistic Regerssion";"DecisionTree";"DecisionTree";]
let cChart  (x:float[])  =
    [Chart.Point(x,[0.1;0.2;0.3;0.4;0.5;0.6;0.7;0.8;0.9;1.0;],Name="Best Occuracy",
        MultiText=labels,
        TextPosition=StyleParam.TextPosition.TopRight)] |> Chart.combine