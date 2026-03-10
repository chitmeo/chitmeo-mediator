// See https://aka.ms/new-console-template for more information
using ChitMeo.Mediator;
using ChitMeo.Module.Example;

using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddMediator();

var provider = services.BuildServiceProvider();

var mediator = provider.GetRequiredService<IMediator>();

var result = await mediator.SendAsync(new HelloRequest());

Console.WriteLine(result);