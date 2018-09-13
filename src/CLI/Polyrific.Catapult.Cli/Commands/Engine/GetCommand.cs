﻿// Copyright (c) Polyrific, Inc 2018. All rights reserved.

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Polyrific.Catapult.Cli.Extensions;
using Polyrific.Catapult.Shared.Service;
using System.ComponentModel.DataAnnotations;

namespace Polyrific.Catapult.Cli.Commands.Engine
{
    [Command(Description = "Get a single engine record")]
    public class GetCommand : BaseCommand
    {
        private readonly ICatapultEngineService _engineService;

        public GetCommand(IConsole console, ILogger<GetCommand> logger, ICatapultEngineService engineService) : base(console, logger)
        {
            _engineService = engineService;
        }

        [Required]
        [Option("-n|--name <NAME>", "Name of the engine", CommandOptionType.SingleValue)]
        public string Name { get; set; }

        public override string Execute()
        {
            string message = string.Empty;
            var engine = _engineService.GetCatapultEngineByName(Name).Result;

            if (engine != null)
            {
                message = engine.ToCliString($"Engine {Name}");
            }
            else
            {
                message = $"Engine {Name} is not found";
            }

            return message;
        }
    }
}
