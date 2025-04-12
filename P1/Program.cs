using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace DiceGame
{
    public class DiceForm : Form
    {
        Button rollButton;
        PictureBox player1Dice;
        PictureBox player2Dice;
        Label resultLabel;
        ComboBox modeSelector;
        HttpClient client = new HttpClient();
        string apiKey = "";

        public DiceForm()
        {
            Text = "Игра в кости";
            Size = new Size(300, 250);

            modeSelector = new ComboBox { Location = new Point(10, 10), Width = 150 };
            modeSelector.Items.AddRange(new[] { "Человек vs Человек", "Человек vs Компьютер" });
            modeSelector.SelectedIndex = 0;
            Controls.Add(modeSelector);

            rollButton = new Button { Text = "Бросить", Location = new Point(170, 10) };
            rollButton.Click += async (s, e) => await RollDice();
            Controls.Add(rollButton);

            player1Dice = new PictureBox { Location = new Point(30, 50), Size = new Size(80, 80) };
            player2Dice = new PictureBox { Location = new Point(160, 50), Size = new Size(80, 80) };
            Controls.Add(player1Dice);
            Controls.Add(player2Dice);

            resultLabel = new Label { Location = new Point(10, 150), AutoSize = true };
            Controls.Add(resultLabel);
        }

        async Task RollDice()
        {
            int d1 = await GetNumber();
            int d2 = modeSelector.SelectedIndex == 0 ? await GetNumber() : new Random().Next(1, 7);

            await Animate(player1Dice, d1);
            await Animate(player2Dice, d2);

            if (d1 > d2) resultLabel.Text = "Игрок 1 выиграл!";
            else if (d2 > d1) resultLabel.Text = "Игрок 2 выиграл!";
            else resultLabel.Text = "Ничья!";
        }

        async Task<int> GetNumber()
        {
            var req = new
            {
                jsonrpc = "2.0",
                method = "generateIntegers",
                @params = new { apiKey = apiKey, n = 1, min = 1, max = 6 },
                id = 1
            };

            var content = new StringContent(JsonSerializer.Serialize(req));
            var res = await client.PostAsync("https://api.random.org/json-rpc/4/invoke", content);
            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("result").GetProperty("random").GetProperty("data")[0].GetInt32();
        }

        async Task Animate(PictureBox box, int value)
        {
            for (int i = 0; i < 5; i++)
            {
                int temp = new Random().Next(1, 7);
                box.Image = Image.FromFile($"dice{temp}.png");
                await Task.Delay(50);
            }
            box.Image = Image.FromFile($"dice{value}.png");
        }

        [STAThread]
        static void Main()
        {
            Application.Run(new DiceForm());
        }
    }
}
