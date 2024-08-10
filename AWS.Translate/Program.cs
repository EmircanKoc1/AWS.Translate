using Amazon.Translate;
using Amazon.Translate.Model;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAWSService<IAmazonTranslate>();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("translate-text", async (
    [FromServices] IAmazonTranslate _amazonTranslate,
    [FromBody] TranslateModel translateModel) =>
{
    if (!(LangCodeIsValid(translateModel.sourceLanguage) && LangCodeIsValid(translateModel.targetLanguage)))
        return Results.BadRequest("invalid lang codes");


    var translateTextRequest = new TranslateTextRequest()
    {
        Text = translateModel.text,
        SourceLanguageCode = translateModel.sourceLanguage,
        TargetLanguageCode = translateModel.targetLanguage
    };


    var translateTextResponse = await _amazonTranslate.TranslateTextAsync(translateTextRequest);

    return Results.Ok(translateTextResponse.TranslatedText);

});

app.MapGet("list-languages", async (
    [FromServices] IAmazonTranslate _amazonTranslate,
    [FromQuery] int resultCount) =>
{
    var listLanguageRequest = new ListLanguagesRequest()
    {
        MaxResults = resultCount,
    };

    var langs = await _amazonTranslate.ListLanguagesAsync(listLanguageRequest);

    return Results.Ok(langs.Languages);

app.Run();


static bool LangCodeIsValid(string langCode)
{
    foreach (var lang in GetLangCodes())
        if (lang.Equals(langCode))
            return true;

    return false;

}

static IEnumerable<string> GetLangCodes()
{
    yield return "en"; // English
    yield return "tr"; // Turkish
    yield return "de"; // German
    yield return "fr"; // French
    yield return "es"; // Spanish
    yield return "it"; // Italian
    yield return "ja"; // Japanese
    yield return "zh"; // Chinese (Simplified)
    yield return "ru"; // Russian
    yield return "ar"; // Arabic
    yield return "pt"; // Portuguese
    yield return "ko"; // Korean
    yield return "fa"; // Persian (Iran)
    yield return "hi"; // Hindi
    yield return "el"; // Greek
    yield return "sv"; // Swedish
    yield return "no"; // Norwegian
    yield return "nl"; // Dutch
    yield return "pl"; // Polish

}

internal record TranslateModel(string sourceLanguage, string targetLanguage, string text);