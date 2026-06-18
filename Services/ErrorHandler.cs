

using System.Diagnostics;
using System.IO;
using Aris.Models;

namespace Aris.Services
{
    public static class ErrorHandler
    {
        public static Result Handler(Exception ex, string context = null)
        {
            Debug.WriteLine($"[{context}] {ex.GetType().Name}: {ex.Message}");

            var message = ex switch
            {
                FileNotFoundException => "File could not be found!. It may have been moved or deleted.",
                UnauthorizedAccessException => "Aris do not have permission to access to this file.",
                IOException => "This file is already in another process. Close it and try again.",
                _ => "An unexpected error occurred. Please try again."
            };

            return Result.Fail(message);
        }
    }
}