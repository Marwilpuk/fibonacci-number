using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;

namespace FibonacciNumber
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Instantiates a new instance of <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fibonacci array result.
        /// </summary>
        int[] Fibonacci;

        /// <summary>
        /// Number of elements in Fibonacci array.
        /// </summary>
        int numberOfDigits;

        /// <summary>
        /// Element in Fibonacci array starting from which other elements will be copied (to left
        /// or right, depending on <see cref="movementNumber"/>).
        /// </summary>
        int startingNumber;

        /// <summary>
        /// Index of <see cref="startingNumber"/> in <see cref="Fibonacci"/> array.
        /// </summary>
        int startingIndex;

        /// <summary>
        /// Absolute value of <see cref="movementNumber"/> is the number of elements that are
        /// copied from <see cref="Fibonacci"/> array. If <see cref="movementNumber"/> is
        /// positive, the elements are copied starting from <see cref="startingIndex"/> to
        /// the right, otherwise the elements are copied to the left.
        /// </summary>
        int movementNumber;

        /// <summary>
        /// Flag that indicates whether the <see cref="Key.Enter"/> has been pressed while the
        /// <see cref="NumberInputBox"/> has been focused.
        /// </summary>
        bool numberInputBoxClicked = false;

        /// <summary>
        /// Flag that indicates whether the <see cref="Key.Enter"/> has been pressed while the
        /// <see cref="StartingNumberBox"/> has been focused.
        /// </summary>
        bool startingNumberBoxClicked = false;

        /// <summary>
        /// Method that is executed on <see cref="NumberInputBox"/> KeyDown event.
        /// Creating a Fibonnaci array for given number of digits in <see cref="NumberInputBox"/>,
        /// Serializing Fibonacci array into JSON,
        /// Calculating elements sum, counting odd and even numbers.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">KeyDown event arguments.</param>
        private void NumberInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            numberInputBoxClicked = true;
            bool inputCheck = int.TryParse(NumberInputBox.Text, out numberOfDigits);

            // Checking if number of Fibonacci elements is valid.
            if (!inputCheck || numberOfDigits <= 0 || numberOfDigits > int.MaxValue)
            {
                MessageBox.Show("Please enter a valid number in range from 1 to 2147483647");
                return;
            }

            Fibonacci = new int[numberOfDigits];
            long sum = 0;
            int evenNumbers = 1, oddNumbers = 0;

            // Setting initial Fibonacci elements.
            Fibonacci[0] = 0;
            if (numberOfDigits > 1)
            {
                Fibonacci[1] = 1;
                sum = 1;
                oddNumbers = 1;
            }

            try
            {
                // Calculating rest of Fibonacci elements, summing them and counting odd and even numbers.
                for (int i = 2; i < numberOfDigits; i++)
                {
                    Fibonacci[i] = Fibonacci[i - 2] + Fibonacci[i - 1];
                    sum += Fibonacci[i];

                    if (sum< 0)
                    {
                        throw new Exception("Overflow occured");
                    }

                    if (Fibonacci[i] % 2 == 0)
                    {
                        evenNumbers++;
                    }
                    else
                    {
                        oddNumbers++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            // Creating object for JSON result
            var jsonResult = new
            {
                count = numberOfDigits,
                sum = sum,
                fibonaciiJson = Fibonacci
            };

            FibonacciTextBox.Text = String.Join(",", Fibonacci);
            JsonBox.Text = JsonConvert.SerializeObject(jsonResult, Formatting.Indented);
            SumBox.Text = sum.ToString();
            EvenBox.Text = evenNumbers.ToString();
            OddBox.Text = oddNumbers.ToString();
        }

        /// <summary>
        /// Method that is executed on <see cref="OrdinalNumberBox"/> KeyDown event.
        /// Outputs number from Fibonacci array on position specified in <see cref="OrdinalNumberBox"/> in <see cref="ElementOutputBox"/>.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">KeyDown event arguments.</param>
        private void OrdinalNumberBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || !numberInputBoxClicked)
            {
                return;
            }

            int ordinalNumber;
            bool inputCheck = int.TryParse(OrdinalNumberBox.Text, out ordinalNumber);

            if (!inputCheck || ordinalNumber <= 0 || ordinalNumber > numberOfDigits)
            {
                MessageBox.Show($"Please enter a valid number in range from 1 to {numberOfDigits}");
                return;
            }

            ElementOutputBox.Text = Fibonacci[ordinalNumber - 1].ToString();
        }

        /// <summary>
        /// Method that is executed on <see cref="StartingNumberBox"/> KeyDown event.
        /// Creating new array starting from <see cref="startingNumber"/>. The number of elements
        /// in new array is the absolute value of <see cref="movementNumber"/>. If
        /// <see cref="movementNumber"/> is positive, the elements are copied starting from
        /// <see cref="startingIndex"/> to the right, otherwise the elements are copied to the left.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">KeyDown event arguments.</param>
        private void StartingNumberBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || !numberInputBoxClicked)
            {
                return;
            }

            startingNumberBoxClicked = true;

            bool inputCheck = int.TryParse(StartingNumberBox.Text, out startingNumber);

            if (!inputCheck || startingNumber > int.MaxValue)
            {
                MessageBox.Show("Please enter valid number");
            }

            if (!Fibonacci.Contains(startingNumber))
            {
                MessageBox.Show($"Please enter a valid number from fibonnaci sequence");
                return;
            }

            // In case that starting number is 1, checking to see which one is starting number for new array.
            if (startingNumber == 1)
            {
                MessageBoxResult dialogResult = MessageBox.Show("Would you like to select first one, if not, second will be selected?", "Choosing digit ", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    startingIndex = 1;
                }
                else if (dialogResult == MessageBoxResult.No)
                {
                    startingIndex = 2;
                }
            }
            else
            {
                startingIndex = Array.IndexOf(Fibonacci, startingNumber);
            }

            // Checking if number of elements of new array is valid.
            if (movementNumber > 0)
            {
                if (movementNumber > numberOfDigits - startingIndex)
                {
                    MessageBox.Show($"Out of range, please define another sequence");
                    return;
                }
            }
            else
            {
                if (Math.Abs(movementNumber) > startingIndex + 1)
                {
                    MessageBox.Show($"Out of range, please define another sequence");
                    return;
                }
            }

            NewArrayBox.Text = String.Join(",", NewFibonacciArray(Fibonacci, movementNumber, startingNumber, startingIndex));
        }

        /// <summary>
        /// Method that is executed on <see cref="MovementBox"/> KeyDown event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">KeyDown event arguments.</param>
        private void MovementBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || !startingNumberBoxClicked)
            {
                return;
            }

            bool inputCheck = int.TryParse(MovementBox.Text, out movementNumber);

            if (!inputCheck || Math.Abs(movementNumber) > numberOfDigits || movementNumber == 0)
            {
                MessageBox.Show($"Please enter a valid number");
                return;
            }

            // Checking if number of elements of new array is valid.
            if (movementNumber > 0)
            {
                if (movementNumber > numberOfDigits - startingIndex)

                {
                    MessageBox.Show($"Out of range, please define another sequence");
                    return;
                }
            }
            else
            {
                if (Math.Abs(movementNumber) > startingIndex + 1)
                {
                    MessageBox.Show($"Out of range, please define another sequence");
                    return;
                }
            }

            NewArrayBox.Text = String.Join(",", NewFibonacciArray(Fibonacci, movementNumber, startingNumber, startingIndex));
        }

        /// <summary>
        /// Creates new array that is a subset of existing array.
        /// </summary>
        /// <param name="originalArray">Original array from which the elements are copied.</param>
        /// <param name="newLength">
        /// Absolute value of newLength is the number of elements that are
        /// copied from originalArray array. If newLength is
        /// positive, the elements are copied starting from originalIndex to
        /// the right, otherwise the elements are copied to the left.
        /// </param>
        /// <param name="originalPoint">
        /// Element in originalArray array starting from which other elements will be copied (to left
        /// or right, depending on newLength).
        /// </param>
        /// <param name="originalIndex">
        /// Index of originalPoint in originalArray.
        /// </param>
        /// <returns>New array that is a subset of existing array.</returns>
        public static int[] NewFibonacciArray(int[] originalArray, int newLength, int originalPoint, int originalIndex)
        {
          
            int[] newFibonacci = new int[Math.Abs(newLength)];
                              
            if (newLength > 0)
            {
                for (int i = 0; i < newLength; i++)
                {
                    newFibonacci[i] = originalArray[originalIndex + i];
                }
            }
            else
            {
                for (int i = 0; i < Math.Abs(newLength); i++)
                {
                    newFibonacci[i] = originalArray[originalIndex + 1 + newLength + i];
                }
            }

            return newFibonacci;
        }
    }
}
