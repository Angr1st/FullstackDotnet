namespace ArgumentParsing

open Argu

type CliArguments =
    | [<Unique>]DbConnectionString of string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | DbConnectionString _ -> "DbConnectionstring to connect to the Db"

type CliArgumentResult =
    {
    DbConnectionString:string
    }

module Parsing =
    let parseArguments args =
        let parser = ArgumentParser<CliArguments> "FindBiggestFile.exe"
    
        let defaultCliArgumentResult =
            {
                DbConnectionString=""
            }

        let toArgContainer arguments =
            let rec parseArguments (list: CliArguments list) (acc:CliArgumentResult) =
                match list with
                | [] -> acc
                | x::xs ->
                    match x with
                    | DbConnectionString z -> {acc with DbConnectionString=z} |> parseArguments xs    
        
            parseArguments arguments defaultCliArgumentResult

        try
            let parsedResults = parser.Parse args
            let results = parsedResults.GetAllResults()
            results
            |> toArgContainer
            |> Ok
        with
        | :? ArguException as ax -> 
            eprintfn "%s" ax.Message
            Error -1
