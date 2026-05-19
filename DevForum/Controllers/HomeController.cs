using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DevForum.Models;

namespace DevForum.Controllers;

// HomeController: Yeh aapki website ka default controller hai.
// Jab koi user sirf base URL (jaise www.mysite.com) par aata hai, toh yeh controller chalta hai.
public class HomeController : Controller
{
    // Index Action: Yeh website ka "Home Page" hai.
    // Jab user "/" ya "/Home/Index" URL par jayega, toh yeh method execute hoga.
    public IActionResult Index()
    {
        // View() ka matlab hai ki yeh "Views/Home/Index.cshtml" file ko dhoondhega aur browser mein render karega.
        return View();
    }

    // Privacy Action: Yeh Privacy Policy page ke liye hai.
    // Iska URL hoga: "/Home/Privacy"
    public IActionResult Privacy()
    {
        // Yeh "Views/Home/Privacy.cshtml" file ko user ko dikhayega.
        return View();
    }

    // Error Handling: Agar app mein koi error aata hai, toh yeh action trigger hota hai.
    // [ResponseCache] attribute: Yeh browser ko bol raha hai ki error page ko cache (save) mat karo, 
    // kyunki error hamesha naya aur updated hona chahiye.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Yeh ek 'ErrorViewModel' banata hai jisme 'RequestId' hoti hai.
        // RequestId se developer ko pata chalta hai ki server par kaunsa specific request fail hua tha.
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}