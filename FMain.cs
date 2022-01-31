using System.Text.Json;

namespace Paraulogic.Tester;

public partial class FMain : Form
{
    private bool forceStop = false;

    public FMain()
    {
        InitializeComponent();
    }

    private async void StartButton_Click(object sender, EventArgs e)
    {
        try
        {
            StartButton.Enabled = false;

            using var client = new HttpClient();
            var response = await client.GetStringAsync("https://www.vilaweb.cat/paraulogic/");
            response = response[response.IndexOf("var t")..];
            response = response[response.IndexOf("\"p\":")..];
            response = response.Substring(4, response.IndexOf(';')).Trim();
            response = response[0..^2];

            var solutions = JsonSerializer.Deserialize<Dictionary<string, string>>(response, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            MessageBox.Show(this, $"5 seconds to start writing {solutions.Count} words! Put focus on browser!", "VamosIshi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            await Task.Delay(5000);
            forceStop = false;

            foreach (string word in solutions.Keys)
            {
                if (forceStop)
                    break;

                SendKeys.Send(word + "\n");
                SendKeys.Flush();

                await Task.Delay(200);
            }

            if (forceStop)
                MessageBox.Show(this, "Stopped forced by focus", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            StartButton.Enabled = true;
        }
    }

    private void FMain_Activated(object sender, EventArgs e)
    {
        forceStop = true;
    }
}
