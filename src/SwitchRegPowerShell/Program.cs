/*
   Простая консольная программа для удобного изменения политики запуска скриптов в PowerShell.

   Данная программа является свободным программным обеспечением, распространяющимся по лицензии MIT.
   Копия лицензии: https://opensource.org/licenses/MIT

   Copyright (c) 2024 Otto
   Автор: Otto
   Версия: 27.06.24
   GitHub страница:  https://github.com/Otto17/SwitchRegPowerShell
   GitFlic страница: https://gitflic.ru/project/otto/switchregpowershell

   г. Омск 2024
*/


using System;               // Библиотека предоставляет доступ к базовым классам и функциональности .NET Framework
using System.Diagnostics;   // Библиотека позволяет получать доступ к информации о процессах, потоках, событиях и выполнении кода приложения
using Microsoft.Win32;      // Библиотека позволяет работать с реестром Windows

namespace SwitchRegPowerShell
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Возможные значения параметра "ExecutionPolicy" (в нижнем регистре)
            string[] validPolicies = { "restricted", "allsigned", "remotesigned", "unrestricted", "bypass", "list", "off" };

            //Если получили аргумент не равный одному и в массиве нет нужного аргумента, тогда выводим справку
            if (args.Length != 1 || Array.IndexOf(validPolicies, args[0].ToLower()) == -1)
            {
                //Вывод справки
                Console.ForegroundColor = ConsoleColor.Blue; // Устанавливаем синий цвет для строкb ниже
                Console.WriteLine("Простая консольная программа для удобного изменения политики запуска скриптов в PowerShell.");

                Console.ForegroundColor = ConsoleColor.Red; // Устанавливаем красный цвет для строк ниже
                Console.WriteLine("Программа изменяет значение строкового параметра \"ExecutionPolicy\" в реестре по пути \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\PowerShell\\1\\ShellIds\\Microsoft.PowerShell\"\n");
                Console.WriteLine("Использование: SwitchRegPowerShell.exe <ExecutionPolicy>\n");

                Console.ForegroundColor = ConsoleColor.Blue; // Устанавливаем синий цвет для строк ниже
                Console.WriteLine("Где <ExecutionPolicy> может быть одним из следующих:");

                Console.ForegroundColor = ConsoleColor.White; // Устанавливаем белый цвет для строк ниже
                Console.WriteLine("Restricted   - запрещает запуск всех скриптов, кроме тех, которые введены напрямую в PowerShell или предварительно загружены.");
                Console.WriteLine("AllSigned    - разрешает выполнение только подписанных скриптов.");
                Console.WriteLine("RemoteSigned - разрешает выполнение всех локальных скриптов, а для удаленных требует подписи.");
                Console.WriteLine("Bypass       - игнорирует все ограничения и разрешает выполнение всех скриптов.");
                Console.WriteLine("Unrestricted - разрешает выполнение всех скриптов без ограничений (уровень полномочий выше, чем у \"Bypass\").");
                Console.WriteLine("List         - выводит список всех установленных политик выполнения скриптов в PowerShell");
                Console.WriteLine("Off          - Восстанавливает политику \"По умолчанию\".\n");

                Console.ForegroundColor = ConsoleColor.Yellow; // Устанавливаем жёлтый цвет для строк ниже
                Console.WriteLine("Автор Otto, г.Омск 2024");
                Console.WriteLine("GitHub страница:  https://github.com/Otto17/SwitchRegPowerShell");
                Console.WriteLine("GitFlic страница: https://gitflic.ru/project/otto/switchregpowershell");
                Console.ResetColor(); // Сбрасываем цвет на стандартный
                return;
            }

            string policy = args[0].ToLower();                                                      // Приводим аргумент к нижнему регистру
            string registryPath = @"SOFTWARE\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell"; // Путь реестра
            string valueName = "ExecutionPolicy";                                                   // Строковое значение в реестре

            try
            {
                if (policy == "list")   // Если получили аргумент "List"
                {
                    //Выполняем в PowerShell команду "Get-ExecutionPolicy -List"
                    ExecutePowerShellCommand("Get-ExecutionPolicy -List");
                    return;
                }

                //Открываем ключ реестра с правами на изменение
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath, true))  // Открытие подраздела реестра в разделе "LocalMachine"
                {
                    if (key == null)    // Если ключ не найден
                    {
                        Console.WriteLine("Ключ реестра не найден.");
                    }
                    else
                    {
                        if (policy == "off")    // Если получили аргумент "Off"
                        {
                            //Удаляем параметр из реестра
                            key.DeleteValue(valueName, false);
                            Console.WriteLine($"Параметр \"{valueName}\" был удален.");
                        }
                        else
                        {
                            //Иначе устанавливаем значение для параметра из аргумента
                            key.SetValue(valueName, policy, RegistryValueKind.String);
                            Console.WriteLine($"Значение \"{valueName}\" изменено на \"{policy}\".");
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Ошибка доступа: убедитесь, что программа запущена с правами администратора.");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка:");
                Console.WriteLine(ex.Message);
            }
        }


        //Метод для выполнения команды в PowerShell из командной строки
        static void ExecutePowerShellCommand(string command)
        {
            using (Process process = new Process()) // Создаём новый процесс
            {
                //Установка параметров для нового процесса
                process.StartInfo.FileName = "powershell";          // Указание исполняемого файла
                process.StartInfo.Arguments = command;              // Аргумент команды
                process.StartInfo.RedirectStandardOutput = true;    // Перенаправление стандартного вывода
                process.StartInfo.RedirectStandardError = true;     // Перенаправление стандартных ошибок
                process.StartInfo.UseShellExecute = false;          // Запрет использования оболочки
                process.StartInfo.CreateNoWindow = true;            // Скрытие окна

                process.Start();    // Запускаем процесс

                string result = process.StandardOutput.ReadToEnd(); // Получаем вывод выполнения команды
                string error = process.StandardError.ReadToEnd();   // Получаем ошибку выполнения команды

                process.WaitForExit();  // Ожидаем завершение выполнения процесса

                //Если вывод не пустой, он выводится на консоль
                if (!string.IsNullOrEmpty(result))
                {
                    Console.WriteLine(result);
                }

                //Если ошибка не пустая, она выводится на консоль
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Ошибка выполнения команды PowerShell:");
                    Console.WriteLine(error);
                }
            }
        }
    }
}
