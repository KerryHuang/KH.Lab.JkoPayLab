// See https://aka.ms/new-console-template for more information
using KH.Lab.JkoPayLab.App;

Console.WriteLine("Hello, World!");

JKOPayService service = new();

var result = await service.CreateOrderAsync();

Console.WriteLine(result);
