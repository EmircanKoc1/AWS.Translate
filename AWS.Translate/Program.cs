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




app.Run();
