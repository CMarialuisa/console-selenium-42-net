using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsoleSelenium.Tests;

[TestClass]
public class BuscaDesafioTest
{
    private const string UrlBase = "https://duckduckgo.com";

    [TestMethod]
    public void Exercicio1_PesquisaBasica_ValidacaoTitulo()
    {
        IWebDriver driver = new ChromeDriver();
        try
        {
            // 1. Navegar até a URL base do DuckDuckGo
            driver.Navigate().GoToUrl(UrlBase);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // 2. Localizar o campo de pesquisa (name="q")
            var campoBusca = wait.Until(d => d.FindElement(By.Name("q")));

            // 3. Digitar "Selenium WebDriver C#" e 4. Pressionar Enter
            string termoPesquisado = "Selenium WebDriver C#";
            campoBusca.SendKeys(termoPesquisado + Keys.Enter);

            // 5. Validar por meio de asserção que o título contém o termo pesquisado
            wait.Until(d => d.Title.Contains(termoPesquisado, StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(
                driver.Title.Contains(termoPesquisado, StringComparison.OrdinalIgnoreCase),
                $"O título '{driver.Title}' não contém o termo esperado '{termoPesquisado}'."
            );
        }
        finally
        {
            driver.Quit();
        }
    }

    [TestMethod]
    public void Exercicio2_Autocompletar_Suggestions()
    {
        IWebDriver driver = new ChromeDriver();
        try
        {
            // 1. Acessar a página inicial do DuckDuckGo
            driver.Navigate().GoToUrl(UrlBase);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // 2. Digitar "C# unit testing" sem pressionar Enter
            var campoBusca = wait.Until(d => d.FindElement(By.Name("q")));
            campoBusca.SendKeys("C# unit testing");

            // 3. Espera explícita (WebDriverWait) até aparecerem pelo menos 3 sugestões no DOM
            var seletorSugestoes = By.CssSelector(".search-suggestions-list li, .sbox-suggestions li, [role='option']");
            var sugestoes = wait.Until(d =>
            {
                var elementos = d.FindElements(seletorSugestoes);
                return elementos.Count >= 3 ? elementos : null;
            });

            string textoTerceiraOpcao = sugestoes![2].Text;

            // 4. Clicar na terceira opção sugerida
            sugestoes[2].Click();

            // 5. Garantir através de asserção que redirecionou para os resultados da opção escolhida
            wait.Until(d => d.Url.Contains("q=", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(
                driver.Url.Contains("q=", StringComparison.OrdinalIgnoreCase) &&
                driver.Title.Contains("C#", StringComparison.OrdinalIgnoreCase),
                $"Não houve redirecionamento correto para a opção '{textoTerceiraOpcao}'. URL atual: {driver.Url}"
            );
        }
        finally
        {
            driver.Quit();
        }
    }

    [TestMethod]
    public void Exercicio3_NavegacaoHistorico_VoltarEAvancar()
    {
        IWebDriver driver = new ChromeDriver();
        try
        {
            // 1. Realizar uma busca inicial por "Automated Testing"
            driver.Navigate().GoToUrl(UrlBase);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var campoBusca = wait.Until(d => d.FindElement(By.Name("q")));
            campoBusca.SendKeys("Automated Testing" + Keys.Enter);

            // 2. Clicar no primeiro link orgânico relevante dos resultados
            var primeiroLinkOrganico = wait.Until(d =>
            {
                var links = d.FindElements(By.CssSelector("a[data-testid='result-title-a'], article h2 a"));
                return links.FirstOrDefault(l => l.Displayed && !string.IsNullOrWhiteSpace(l.GetAttribute("href")));
            });

            primeiroLinkOrganico!.Click();

            // 3. Validar que a página externa carregou verificando driver.Url
            wait.Until(d => !d.Url.Contains("duckduckgo.com", StringComparison.OrdinalIgnoreCase));
            Assert.IsFalse(
                driver.Url.Contains("duckduckgo.com", StringComparison.OrdinalIgnoreCase),
                $"Era esperado navegar para uma página externa, mas permaneceu em: {driver.Url}"
            );

            // 4. Utilizar o comando Back() do Selenium para retornar aos resultados
            driver.Navigate().Back();

            // 5. Validar que a página de resultados foi restaurada corretamente
            wait.Until(d => d.Url.Contains("duckduckgo.com", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(
                driver.Url.Contains("duckduckgo.com", StringComparison.OrdinalIgnoreCase),
                $"A página de resultados do DuckDuckGo não foi restaurada. URL atual: {driver.Url}"
            );
        }
        finally
        {
            driver.Quit();
        }
    }
}