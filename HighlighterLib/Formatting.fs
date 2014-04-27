﻿namespace CSHighlighter

module Formatting =

    open System
    open System.Text
    open Analysis
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax
    open Microsoft.CodeAnalysis.Text
    open System.Net

    type HtmlElement =
        | Span of id: string option * contents: string * cssClasses: string array
        | Anchor of href: string * contents: string * cssClasses: string array
        | Literal of text: string

    let formatHtmlElement (htmlEle:HtmlElement) =
        let combineClasses (c: string array) = String.Join(" ", c)
        match htmlEle with
        | Span (id, contents, classes) -> 
            let css = combineClasses classes
            let encoded = WebUtility.HtmlEncode contents
            match id with
            | Some idAttr -> sprintf @"<span id=""%s"" class=""%s"">%s</span>" idAttr css encoded
            | None -> sprintf @"<span class=""%s"">%s</span>" css encoded
        | Anchor (href, contents, classes) -> 
            let css = combineClasses classes
            let encoded = WebUtility.HtmlEncode contents
            sprintf @"<a href=""%s"" class=""%s"">%s</a>" href css encoded
        | Literal text -> WebUtility.HtmlEncode text

    let locationToClassName (location:Location) = 
        match location.Kind with
            | LocationKind.SourceFile -> sprintf "loc%d_%d" location.SourceSpan.Start location.SourceSpan.End
            | _ -> failwith "Trying to get a css class name for token that is not in a source file."

    let symbolIdName (symbol:ISymbol) =
        let loc = Seq.head symbol.Locations
        locationToClassName loc

    let tokenClassName (tok:SyntaxToken) =
        let loc = tok.GetLocation()
        locationToClassName loc

    let locationClassName ele = 
        match ele with
        | NamedTypeDeclaration tok -> Some <| tokenClassName tok
        | LocalVariableDeclaration (tok, _) -> Some <| tokenClassName tok
        | FieldDeclaration tok -> Some <| tokenClassName tok
        | ParameterDeclaration tok -> Some <| tokenClassName tok
        | PropertyDeclaration tok -> Some <| tokenClassName tok
        | _ -> None

    let generateCss (eles: OutputElement array) =
        let hoverClasses = 
            seq {
                for hClass in Array.choose locationClassName eles do
                    yield sprintf ".%s:hover { background-color: aqua; }%s" hClass Environment.NewLine
            }
        String.Join(Environment.NewLine, hoverClasses)

    let htmlFormat (eles: OutputElement array) =

        let intoSpan spanClass id text = Span (id, text, spanClass)
        let intoLiteralSpan spanClass = intoSpan spanClass None
        let intoHref spanClass ref text = Anchor (sprintf "#%s" ref, text, spanClass)

        let comment = 
            intoLiteralSpan [|"comment"|]
        let keyword = 
            intoLiteralSpan [|"keyword"|]
        let ident = 
            intoLiteralSpan [|"identifier"|]
        let stringLiteral = 
            intoLiteralSpan [|"stringLiteral"|]
        let numericLiteral = 
            intoLiteralSpan [|"numericLiteral"|]
        let region text = 
            intoLiteralSpan [|"region"|] (text + Environment.NewLine)

        let sourceReference referenceClass tok sym =
            let c = tokenClassName tok
            let href = symbolIdName sym
            intoHref [|referenceClass; href|] href

        let localRef = sourceReference "localRef"
        let fieldRef = sourceReference "fieldRef"
        let paramRef = sourceReference "paramRef"
        let propRef = sourceReference "propRef"

        let sourceDeclaration declClass tok =
            let c = tokenClassName tok
            intoSpan [|declClass; c|] (Some c)

        let propDecl = sourceDeclaration "propDecl"
        let paramDecl = sourceDeclaration "paramDecl"
        let fieldDecl = sourceDeclaration "fieldDecl"
        let localDecl = sourceDeclaration  "localDecl"

        let toStr (tok:SyntaxToken) =
            tok.ToString()

        let formatter ele = 
            match ele with
            | Unformatted tok -> Literal <| toStr tok
            | Keyword tok -> keyword <| toStr tok
            | Identifier tok -> Literal <| toStr tok
            | NamedTypeDeclaration tok -> ident <| toStr tok
            | NamedTypeReference (tok, sym) -> ident <| toStr tok
            | StringLiteral tok -> stringLiteral <| toStr tok
            | NumericLiteral tok -> numericLiteral <| toStr tok
            | LocalVariableDeclaration (tok, sym) -> localDecl tok <| toStr tok
            | LocalVariableReference (tok, sym) -> localRef tok sym <| toStr tok
            | FieldDeclaration tok -> fieldDecl tok <| toStr tok
            | FieldReference (tok, sym) -> fieldRef tok sym <| toStr tok
            | ParameterDeclaration tok -> paramDecl tok <| toStr tok
            | ParameterReference (tok, sym) -> paramRef tok sym <| toStr tok
            | PropertyDeclaration tok -> propDecl tok <| toStr tok
            | PropertyReference (tok, sym) -> propRef tok sym <| toStr tok
            | Trivia s -> 
                match s with
                | TriviaElement.BeginRegion s -> region <| s.ToString()
                | TriviaElement.Comment s -> comment <| s.ToString()
                | TriviaElement.EndRegion s -> region <| s.ToString()
                | TriviaElement.NewLine -> Literal Environment.NewLine
                | TriviaElement.UnformattedTrivia s -> Literal <| s.ToString()
                | TriviaElement.Whitespace s -> Literal <| s.ToString()
            //| _ -> failwith "DONT KNOW HOW TO FORMAT"
        
        let htmlOutput = 
            let out = new StringBuilder()
            eles
            |> Array.map formatter
            |> Array.iter 
                (fun h -> 
                    formatHtmlElement h
                    |> out.Append
                    |> ignore)

            out.ToString();
        htmlOutput
