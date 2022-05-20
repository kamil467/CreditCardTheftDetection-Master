namespace PlotlyNetHelper
open Plotly.NET

module Say =

  let stackedBar (fraudTran:int32, validTran:int32)=
    [
   
        //Chart.StackedBar(100,1,Name="old");
       // Chart.StackedBar([90; 10;],Name="Fradulent Transaction")
       Chart.StackedColumn([fraudTran;validTran],["Fraud-"+fraudTran.ToString();"Valid-"+validTran.ToString()],Name="new",MarkerColor = Color.fromColors
       [ 
            Color.fromKeyword Red
            Color.fromKeyword Green
            
       ]

       )

     
    ]
    |> Chart.combine
    





    
