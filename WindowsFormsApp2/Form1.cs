using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public class ExpressionHandler
    {
        private Dictionary<string, Operator> _operators = new Dictionary<string, Operator>()
    {
        { "+", new Operator("+", 1, true) },
        { "-", new Operator("-", 1, true) },
        { "*", new Operator("*", 2, true) },
        { "/", new Operator("/", 2, true) },
        { "^", new Operator("^", 3, true) },
    };

        public double Calculate(string input)
        {
            // Use the ShuntingYard algorithm to parse the input string
            List<string> postfix = ShuntingYard(input);

            // Use a stack to evaluate the postfix expression
            Stack<double> stack = new Stack<double>();
            foreach (string token in postfix)
            {
                if (double.TryParse(token, out double number))
                {
                    // If the token is a number, push it onto the stack
                    stack.Push(number);
                }
                else if (_operators.ContainsKey(token))
                {
                    // If the token is an operator, pop the required number of operands from the stack
                    // and evaluate the operation
                    Operator op = _operators[token];
                    double secondOperand = stack.Pop();
                    double firstOperand = stack.Pop();
                    stack.Push(op.Execute(firstOperand, secondOperand));
                }
            }

            // Return the result of the calculation
            return stack.Peek();
        }

        private List<string> ShuntingYard(string input)
        {
            // Initialize the output queue and the operator stack
            List<string> output = new List<string>();
            Stack<Operator> operatorStack = new Stack<Operator>();

            // Split the input string into tokens
            string[] tokens = input.Split(' ');

            // Iterate through the tokens
            foreach (string token in tokens)
            {
                // If the token is a number, add it to the output queue
                if (double.TryParse(token, out double number))
                {
                    output.Add(token);
                }
                else if (_operators.ContainsKey(token))
                {
                    // If the token is an operator, compare its precedence with the top operator on the stack
                    Operator tokenOperator = _operators[token];
                    if (operatorStack.Count > 0)
                    {
                        Operator topOperator = operatorStack.Peek();
                        if (topOperator.Precedence < tokenOperator.Precedence ||
                            (topOperator.Precedence == tokenOperator.Precedence && tokenOperator.IsRightAssociative))
                        {
                            // If the top operator has lower precedence or equal precedence with right associativity,
                            // pop it from the stack and add it to the output queue
                            operatorStack.Pop();
                            output.Add(topOperator.Symbol);
                        }
                    }

                    // Push the new operator onto the stack
                    operatorStack.Push(tokenOperator);
                }
                else if (token == "(")
                {
                    // If the token is an opening parenthesis, push it onto the operator stack
                    operatorStack.Push(new Operator("(", 0, false));
                }
                else if (token == ")")
                {
                    // If the token is a closing parenthesis, pop operators from the stack
                    // and add them to the output queue until the corresponding opening parenthesis is found
                    while (operatorStack.Count > 0)
                    {
                        Operator topOperator = operatorStack.Peek();
                        if (topOperator.Symbol == "(")
                        {
                            // Discard the opening parenthesis
                            operatorStack.Pop();
                            break;
                        }
                        else
                        {
                            // Pop the operator from the stack and add it to the output queue
                            operatorStack.Pop();
                            output.Add(topOperator.Symbol);
                        }
                    }
                }
            }

            // Pop the remaining operators from the stack and add them to the output queue
            while (operatorStack.Count > 0)
            {
                Operator topOperator = operatorStack.Pop();
                output.Add(topOperator.Symbol);
            }

            // Return the postfix expression
            return output;
        }

        private class Operator
        {
            public Operator(string symbol, int precedence, bool isRightAssociative)
            {
                Symbol = symbol;
                Precedence = precedence;
                IsRightAssociative = isRightAssociative;
            }

            public string Symbol { get; private set; }
            public int Precedence { get; private set; }
            public bool IsRightAssociative { get; private set; }

            public double Execute(double left, double right)
            {
                switch (Symbol)
                {
                    case "+":
                        return left + right;
                    case "-":
                        return left - right;
                    case "*":
                        return left * right;
                    case "/":
                        return left / right;
                    case "^":
                        return Math.Pow(left, right);
                    default:
                        throw new InvalidOperationException("Unknown operator");
                }
            }
        }
    }
}
