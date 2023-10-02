using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic.Devices;
using OpenHardwareMonitor.Hardware;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        OpenHardwareMonitor.Hardware.Computer computerHardwareMonitor = new OpenHardwareMonitor.Hardware.Computer();
        computerHardwareMonitor.CPUEnabled = true;
        computerHardwareMonitor.GPUEnabled = true;
        computerHardwareMonitor.Open();

        while (true)
        {
            ulong totalPhysicalMemoryGB = GetTotalPhysicalMemoryGB();
            float availableMemoryGB = GetAvailableMemoryGB();
            float memoryUsagePercentage = (1 - (availableMemoryGB / totalPhysicalMemoryGB)) * 100;

            float cpuUsage = GetCpuUsage(computerHardwareMonitor);
            float gpuUsage = GetGpuUsage(computerHardwareMonitor);

            RenderUsage(memoryUsagePercentage, availableMemoryGB, cpuUsage, gpuUsage);
            Thread.Sleep(1000);
        }
    }

    private static void RenderUsage(float memoryUsagePercentage, float availableMemoryGB, float cpuUsage, float gpuUsage)
    {
        const int totalBars = 50;
        int filledMemoryBars = (int)Math.Round(memoryUsagePercentage / 100 * totalBars);
        int filledCpuBars = (int)Math.Round(cpuUsage / 100 * totalBars);
        int filledGpuBars = (int)Math.Round(gpuUsage / 100 * totalBars);

        Console.SetCursorPosition(0, 0);
        Console.ResetColor();
        Console.Write("Memory Usage: ");
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        for (int i = 0; i < filledMemoryBars; i++)
        {
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.Write(" ");
            Console.ResetColor();
        }
        for (int i = filledMemoryBars; i < totalBars; i++)
        {
            Console.Write(" ");
        }
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.ResetColor();
        Console.Write("]");
        Console.Write($" {memoryUsagePercentage:F2}% - {availableMemoryGB:F2} GB");

        Console.SetCursorPosition(0, 1);
        Console.Write("CPU Usage:    ");
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        for (int i = 0; i < filledCpuBars; i++)
        {
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.Write(" ");
            Console.ResetColor();
        }
        for (int i = filledCpuBars; i < totalBars; i++)
        {
            Console.Write(" ");
        }
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.ResetColor();
        Console.Write("]");
        Console.Write($" {cpuUsage:F2}%");

        Console.SetCursorPosition(0, 2);
        Console.Write("GPU Usage:    ");
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        for (int i = 0; i < filledGpuBars; i++)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write(" ");
            Console.ResetColor();
        }
        for (int i = filledGpuBars; i < totalBars; i++)
        {
            Console.Write(" ");
        }
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.ResetColor();
        Console.Write("]");
        Console.ResetColor();
        Console.Write($" {gpuUsage:F2}%");
    }
    //MEOW
    private static ulong GetTotalPhysicalMemoryGB()
    {
        ulong ramGB = 0;

        try
        {
            ulong ramMB = (ulong)(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024));
            ramGB = ramMB / 1024;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return ramGB;
    }

    private static float GetAvailableMemoryGB()
    {
        using (var memoryCounter = new PerformanceCounter("Memory", "Available MBytes"))
        {
            float availableMemoryMB = memoryCounter.NextValue();
            float availableMemoryGB = availableMemoryMB / 1024; 
            return availableMemoryGB;
        }
    }

    private static float GetCpuUsage(OpenHardwareMonitor.Hardware.Computer computer)
    {
        float cpuUsage = 0;

        if (computer != null && computer.Hardware.Length > 0)
        {
            var cpu = computer.Hardware[0];
            cpu.Update();

            foreach (var sensor in cpu.Sensors)
            {
                if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
                {
                    cpuUsage = sensor.Value ?? 0;
                    break;
                }
            }
        }

        return cpuUsage;
    }

    private static float GetGpuUsage(OpenHardwareMonitor.Hardware.Computer computer)
    {
        float gpuUsage = 0;

        if (computer != null && computer.Hardware.Length > 1)
        {
            var gpu = computer.Hardware[1];
            gpu.Update();

            foreach (var sensor in gpu.Sensors)
            {
                if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("GPU Core"))
                {
                    gpuUsage = sensor.Value ?? 0;
                    break;
                }
            }
        }

        return gpuUsage;
    }
}
