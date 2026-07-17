public class MathCaptchaModel
{
    public string Question { get; set; }
    public int Result { get; set; }
}
public static class CaptchaHelper
{
    public static MathCaptchaModel Generate()
    {
        var random = new Random();
        int a = random.Next(1, 11); // Angka 1-10
        int b = random.Next(1, 11);
        string[] operators = { "+", "-", "*" };
        string op = operators[random.Next(operators.Length)];

        int result = op switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            _ => 0
        };

        return new MathCaptchaModel { Question = $"{a} {op} {b} =", Result = result };
    }
}