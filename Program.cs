using Spectre.Console;
using System;
using FileTransfer;
using System.Timers;
using FileTransfer.Connectors;
using System.IO;
using System.CommandLine;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace FileTransfer
{
    internal class Program
    {
        static async Task Main(string[] args)
        { 
            var rootCommand = new RootCommand("eft - Extreme File Transfer");

            var delayOption = new Option<int>(
                name: "--delay",
                description: "Delay between push requests in seconds.",
                getDefaultValue: () => 2);
            rootCommand.AddGlobalOption(delayOption);

            var replayOption = new Option<int>(
                name: "--replay",
                description: "Number of replays to attempt.",
                getDefaultValue: () => 3);
            rootCommand.AddGlobalOption(replayOption);

            var recurseOption = new Option<bool>(
                name: "--recursive",
                description: "Recursive directory processing.",
                getDefaultValue: () => false);
                rootCommand.AddGlobalOption(recurseOption);

            var logLevelOption = new Option<int>(
                name: "--logging",
                description: "Logging level for the process.",
                getDefaultValue: () => 2);
                rootCommand.AddGlobalOption(logLevelOption);

            var connectionArgument = new Argument<string>
                (name: "connection",
                description: "The connection type to use (file, sftp).",
                getDefaultValue: () => "sharepoint");

            var actionArgument = new Argument<string>
                (name: "action",
                description: "The action to apply (push, move).",
                getDefaultValue: () => "push");

            var pathArgument= new Argument<string>(
                name: "path",
                description: "The directory to be processed.",
                getDefaultValue: () => "c:/temp");

            var queryArgument = new Argument<string>(
                name: "query",
                description: "Connection query string for the connector used.",
                getDefaultValue: () => "");

            rootCommand.Add(connectionArgument);
            rootCommand.Add(actionArgument);
            rootCommand.Add(pathArgument);
            rootCommand.Add(queryArgument);

            rootCommand.SetHandler((connectionArg, actionArg, pathArg, queryArg, delayOpt, replayOpt, recurseOpt, logOpt) =>
            {
           
                Configuration configuration = new Configuration
                {
                    Command = connectionArg,
                    Action = actionArg,
                    FromPath = pathArg,
                    ToPath = queryArg,
                    ReplayCount = replayOpt,
                    Delay = delayOpt,
                    LogLevel = logOpt,
                    Interactive = false,
                    Query = "*.*",
                    Recurisve = recurseOpt
                };

                if (Path.HasExtension(pathArg))
                {
                    configuration.Query = "*" + Path.GetExtension(pathArg);
                    configuration.FromPath = Path.GetDirectoryName(pathArg);
                }
          
                ProcessFiles(configuration);
            },
            connectionArgument, actionArgument, pathArgument, queryArgument, delayOption, replayOption, recurseOption, logLevelOption);

            await rootCommand.InvokeAsync(args);
        }

        static public void ProcessFiles(Configuration configuration)
        {
            Logging log = new Logging();
            Terminal terminal = new Terminal();
            TaskLocker locker = new TaskLocker(configuration.FromPath);
            Encoder encoder = new Encoder();
            DirectoryService directoryService = new DirectoryService();
            int replayCounter = 0;

            if (locker.IsLocked())
            {
                log.WriteLine("Process Already Locked - Exiting", configuration.LogLevel);
                return;
            }

            locker.Lock();
            log.WriteLine("Process Locked", configuration.LogLevel);

            Connector connector = null;

            switch (configuration.Command.ToLower())
            {
                case "file":
                    connector = new FileSystemConnector();
                    break;

                case "trim":
                    connector = new TrimConnector();
                    break;

                case "sharepoint":
                    connector = new SharePointConnector();
                    break;

                case "directus":
                    connector = new DirectusConnector();
                    break;

                case "sftp":
                    connector = new SftpConnector();
                    break;

                default:
                    break;
            }

            if (connector == null)
            {
                locker.Unlock();
                return;
            }

            connector.Configuration = configuration;
            connector.log = log;

            if (configuration.Interactive)
                terminal.WriteLine(configuration.ToString());

            FileInfo[] files = directoryService.List(configuration.FromPath, configuration.Query, configuration.Recurisve);

            foreach (FileInfo file in files)
            {
                if (!file.Name.StartsWith(".xft_") & !file.Directory.Name.StartsWith(".xft_"))
                {
                    switch (configuration.Action.ToLower())
                    {
                        case "push":
                            while (!connector.Push(file, configuration.ToPath) & replayCounter < configuration.ReplayCount)
                            {
                                Thread.Sleep(1000 * configuration.Delay);
                                replayCounter++;
                                log.WriteLine("Replay(" + replayCounter.ToString() + ") " + file.FullName);
                            }
                            if (replayCounter == configuration.ReplayCount)
                            {
                                if (!directoryService.Exists(file.DirectoryName + "/.xft_error_que"))
                                {
                                    directoryService.Create(file.DirectoryName + "/.xft_error_que");
                                }
                                file.MoveTo(file.DirectoryName + "/.xft_error_que/" + file.Name);
                            }
                            replayCounter = 0;
                            break;
                    }
                }

            }

            locker.Unlock();
            log.WriteLine("Process Unlocked", configuration.LogLevel);
            return;
        }
    }
}
