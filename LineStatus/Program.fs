// Learn more about F# at http://fsharp.org

open System
open FSharp.Data

open System.Threading
open System.Threading.Tasks
open System.Diagnostics

type Document = XmlProvider<"LineStatus.xml">

let printDateTime () =
    System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:fff") |> printfn "%s"

let millisecondsUntilNextMinute () =
    ((60 - System.DateTime.Now.Second) * 1000) + System.DateTime.Now.Millisecond

let retrieveLineStatuses () =
    let details =
        Document.Load("http://cloud.tfl.gov.uk/TrackerNet/LineStatus").LineStatuses
        |> Array.map (fun lineStatus -> (lineStatus.Line.Name, lineStatus.Status.Description))

    let (namePadding, descriptionPadding) =
        let names, descriptions = details |> Array.unzip
        let max (arr:string []) = arr |> Array.maxBy (fun x -> x.Length)
        (max names).Length, (max descriptions).Length

    let applyPadding ((str1:String),(str2:String)) =
        (str1.PadRight(namePadding),
         str2.PadRight(descriptionPadding))

    printfn "╔═%s═╦═%s═╗" (String('═', namePadding)) (String('═', descriptionPadding))

    details
    |> Array.map applyPadding
    |> Array.iter (fun (x,y) -> printfn "║ %s ║ %s ║" x y)

    printfn "╚═%s═╩═%s═╝" (String('═', namePadding)) (String('═', descriptionPadding))

while true do
    printDateTime ()
    retrieveLineStatuses ()
    millisecondsUntilNextMinute ()
    |> Thread.Sleep