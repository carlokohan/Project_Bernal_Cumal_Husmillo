using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace Lex_BurnzZ
{

    public partial class Form1 : Form
    {
		int index = 0;


        public Form1()
        {
            InitializeComponent();
            
            
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        /* processes the code when the Analyze button is clicked */
        private void buttonAnalyze_Click(object sender, EventArgs e)
        {

            // genereal List handlers
            List<int> indexes = new List<int>();
            List<String> toks_new = new List<String>();

            // for the OBTW keyword
            Boolean obtwFlag = false;

            // general handler for strings
            String temp = System.String.Empty;

            // flag if kthxbye is found
            Boolean kthxbyeFlag = false;

            // flag if any error is found
            Boolean errorFlag = false;

            // line number
            int lineNum = 1;

            // gets code line by line and trimming whitespaces at the same time
            String[] codelines = splitter(boxCode.Text.Trim());
            List<String> toks = new List<String>();

            // clears the previous entries in Lex, symbol table and console
            LexTable.Rows.Clear();
            SymbolTable.Rows.Clear();
            Console.Clear();
            boxConsole.Text = "";

            // adds to List<String> toks each line entry of code
            foreach (String item in codelines) {
                toks.Add(item.Trim());
            }

            
            
            
            // removes OBTW multi-line comments
            foreach (String x in toks) {
                indexes = findIndexSubstring(x, "OBTW");

                if (indexes.Count != 0 && obtwFlag == false && x.Length > 4) {
                    toks_new.Add(x.Substring(0, indexes[0]));
                    obtwFlag = true;
                }
                else if (obtwFlag == false)
                    toks_new.Add(x);
                else if (obtwFlag == true ) {
                    indexes = findIndexSubstring(x, "TLDR");

                    if (indexes.Count != 0)
                    {

                        // TLDR is the first word
                        if (indexes[0] == 0 && x.Length != 4)
                        {
                            toks_new.Add(x.Substring(4).Trim());
                            obtwFlag = false;
                        }
                        // TLDR is in the middle
                        else if (indexes[0] != 0 && x.Length > 4)
                        {
                            toks_new.Add(x.Substring(indexes[0] + 4).Trim());
                            obtwFlag = false;
                            continue;
                        }
                        // TLDR is the only word
                        else if (indexes[0] == 0 && x.Length == 4)
                        {
                            obtwFlag = false;
                        }


                        toks_new.Add(x.Substring(0, x.Length - (x.Length - indexes[0])));
                        obtwFlag = false;
                    }
                    else if (obtwFlag == false)
                        toks_new.Add(x);
                }
                
            }

            // removes BTW <comments> <comments> ....
            toks.Clear(); // clears all entries from the List<String>
            indexes.Clear();
            foreach (String x in toks_new)
            {

                indexes = findIndexSubstring(x, "BTW");

                // found a BTW keyword in the current line
                if (indexes.Count != 0)
                {
                    toks.Add(x.Substring(0, x.Length - (x.Length - indexes[0])));
                }
                else toks.Add(x);
            }



            // splits up soft-command breaks by a ,
            toks_new.Clear();
            indexes.Clear();
            lineNum = 1;
            foreach (String x in toks.ToList()) {
                if (Regex.IsMatch(x, @",$")) {
                    boxConsole.Text += "ANG TANGA @ line number: " + lineNum + '\n';
                    return;
                }
                else if (Regex.IsMatch(x, @" , ")) {
                    codelines = Regex.Split(x, ",");

                    foreach (String y in codelines)
                    {
                        int i = toks_new.Count;
                        toks_new.Insert(i, y.Trim());
                        lineNum++;
                    }

                }
                else {
                    toks_new.Add(x);
                    lineNum++;
                }
                
            }

            toks = new List<String>(toks_new);


            /* LEX ALGO PROPER */

            // check for boundary HAI
            lineNum = 1;
            if (Regex.IsMatch(toks[0], @"HAI")) {

                // add token tag for HAI and removes HAI from list
                LexTable.Rows.Add(toks[0], "BOUNDARY_START");
                toks.Remove(toks[0]);
                lineNum++;

                
				index = 0;//global var
                SymbolTable.Rows.Add("IT","GLOBAL","null");

                do {

                    // HAI is only the keyword
                    if (toks.Count == 0)
                    {
                        errorFlag = true;
                        break;
                    }

                    // disregards whitepspaces between lines
                    while (Regex.IsMatch(toks[0], @"^[ ]+") || toks[0] == System.String.Empty) {
                        toks.Remove(toks[0]);
                        lineNum++;
                    }
                    
                    if (Regex.IsMatch(toks[0], @"^VISIBLE\s+.*")) {
                        errorFlag = checkIO_print(toks);
                        toks.Remove(toks[0]);
                        if (errorFlag == false) lineNum++;
                    }
                    else if (Regex.IsMatch(toks[0], @"^GIMMEH\s+.*")) {
                        errorFlag = checkIO_scan(toks);
                        toks.Remove(toks[0]);
                        if (errorFlag == false) lineNum++;
                    }
                    else if (Regex.IsMatch(toks[0], @"^I HAS A\s+.*")) {
                        errorFlag = variable_declaration(toks);
                        toks.Remove(toks[0]);
                        if (errorFlag == false) lineNum++;
                    }
                    else if (Regex.IsMatch(toks[0], @"\bR\b")) {
                        errorFlag = variable_assignment(toks);
                        toks.Remove(toks[0]);
                        if (errorFlag == false) lineNum++;
                    }
                    else if (Regex.IsMatch(toks[0], @"^SUM OF") || Regex.IsMatch(toks[0], @"^DIFF OF") ||
                            Regex.IsMatch(toks[0], @"^PRODUKT OF") || Regex.IsMatch(toks[0], @"^QUOSHUNT OF") ||
                            Regex.IsMatch(toks[0], @"^MOD OF") || Regex.IsMatch(toks[0], @"^BIGGR OF") ||
                            Regex.IsMatch(toks[0], @"^SMALLR OF"))
                    {
                        errorFlag = checkOperatorsMath(toks[0]);
                        toks.Remove(toks[0]);
                        if (errorFlag == false) lineNum++;
                    }
                    else if (Regex.IsMatch(toks[0], @"^BOTH OF") || Regex.IsMatch(toks[0], @"^EITHER OF") ||
                             Regex.IsMatch(toks[0], @"^WON OF") || Regex.IsMatch(toks[0], @"^NOT OF") ||
                             Regex.IsMatch(toks[0], @"^ALL OF") || Regex.IsMatch(toks[0], @"^ANY OF"))
                    {
                        errorFlag = checkOperatorsBoolean(toks[0]);
                        toks.Remove(toks[0]);
                        if (errorFlag == false) lineNum++;
                    }
                    else if (Regex.IsMatch(toks[0], @"^BOTH SAEM") || Regex.IsMatch(toks[0], @"^DIFFRINT"))
                    {
                        errorFlag = checkOperatorsComparison(toks[0]);
                        toks.Remove(toks[0]);
                        if (errorFlag == false) lineNum++;
                    }
                    else if (Regex.IsMatch(toks[0], @"KTHXBYE"))
                    {
                        kthxbyeFlag = true;
                        LexTable.Rows.Add(toks[0], "BOUNDARY_END");
                        toks.Remove(toks[0]);
                        if (errorFlag == false) lineNum++;
                    }
                    else
                    {
                        errorFlag = true;
                    }

                //INTERPRET
                    interpret(LexTable, SymbolTable);
                    index++;
                } while(kthxbyeFlag == false && errorFlag == false);
                
            }
            else boxConsole.Text += "ANG TANGA @ line number: " + lineNum + '\n';

            if (errorFlag == true)
                boxConsole.Text += "ANG TANGA @ line number: " + lineNum + '\n';

            

        } //end of buttonAnalyze_Click()

        private void interpret(DataGridView LexTable, DataGridView SymbolTable)
        {
            string lex = LexTable.Rows[index].Cells[0].Value.ToString();
            string tok = LexTable.Rows[index].Cells[1].Value.ToString();

            if (tok == "VARIABLE_DECLARATION")
            {
                index++;
                //lex will be var name
                lex = LexTable.Rows[index].Cells[0].Value.ToString();
                tok = LexTable.Rows[index].Cells[1].Value.ToString();

                index++;
                string lex2 = LexTable.Rows[index].Cells[0].Value.ToString();
                string tok2 = LexTable.Rows[index].Cells[1].Value.ToString();
                //if the next token is ITZ
                if (tok2 == "VARIABLE_DECLARATION_VALUE_ASSIGNMENT")
                {
                    index++;//the value assigned to var name
                    string lex3 = LexTable.Rows[index].Cells[0].Value.ToString();
                    string tok3 = LexTable.Rows[index].Cells[1].Value.ToString();

                    if (tok3 == "NUMBR_LITERAL")
                    {
                        SymbolTable.Rows.Add(lex, "NUMBR_LITERAL", lex3);
                    }
                    else if (tok3 == "NUMBAR_LITERAL")
                    {
                        SymbolTable.Rows.Add(lex, "NUMBAR_LITERAL", lex3);
                    }
                    else if (tok3 == "YARN_LITERAL")
                    {
                        SymbolTable.Rows.Add(lex, "YARN_LITERAL", lex3);
                    }
                }
                else
                {
                    SymbolTable.Rows.Add(lex, "untyped", "null");
                    index--;
                }

                /* CODE FOR CHANGING THE VALUE OR TYPE 
                String searchValue = "IT";
                int rowIndex = -1;
                foreach(DataGridViewRow row in SymbolTable.Rows)
                     {
                            if( row.Cells[0].Value.ToString().Equals(searchValue))
                            {
                                rowIndex = row.Index;
                                break;
                            }
                        } 
                 SymbolTable.Rows.Remove(SymbolTable.Rows[rowIndex]);

                 SymbolTable.Rows.Add("IT", "string", "haha");
                */
            }
            else if (tok == "IO_PRINT")
            {
                index++;
                string lex2 = LexTable.Rows[index].Cells[0].Value.ToString();
                string tok2 = LexTable.Rows[index].Cells[1].Value.ToString();

                if (tok2 == "VARIABLE_NAME")
                {
                    // String searchValue = lex2;
                    int rowIndex = -1;
                    foreach (DataGridViewRow row in SymbolTable.Rows)
                    {
                        string str = (string)row.Cells[0].Value;
                        if (str == lex2)
                        {
                            rowIndex = row.Index;
                            Console.Write(Convert.ToString(row.Cells[2].Value) + "\n");
                            break;
                        }
                    }
                }
                else if (tok2 == "YARN_LITERAL")
                {
                    Console.Write(lex2 + "\n");
                }
                /*else if(Regex.IsMatch(tok2,@""))
                {
                    int rowIndex = -1;
                    foreach (DataGridViewRow row in SymbolTable.Rows)
                    {
                        string str = (string)row.Cells[0].Value;
                        if (str == "IT")
                        {
                            rowIndex = row.Index;
                            Console.Write(Convert.ToString(row.Cells[2].Value) + "\n");
                            break;
                        }
                    }
                }*/
            }
            else if (tok == "IO_SCAN")
            {
                string scan = Console.ReadLine();
                string scantype;
                index++;//var name
                string lex2 = LexTable.Rows[index].Cells[0].Value.ToString();
                string tok2 = LexTable.Rows[index].Cells[1].Value.ToString();

                if (Regex.IsMatch(scan, @"^-?[0-9]+.[0-9]+"))
                    scantype = "NUMBAR_LITERAL";
                else if (Regex.IsMatch(scan, @"^-?[0-9]+"))
                    scantype = "NUMBR_LITERAL";
                else
                    scantype = "YARN_LITERAL";

                String searchValue = lex2;
                int rowIndex = -1;
                foreach (DataGridViewRow row in SymbolTable.Rows)
                {
                    string str = (string)row.Cells[0].Value;
                    if (str == lex2)
                    {
                        rowIndex = row.Index;
                        break;
                    }
                }
                if (rowIndex > -1)
                {
                    SymbolTable.Rows.Remove(SymbolTable.Rows[rowIndex]);
                    SymbolTable.Rows.Add(lex2, scantype, scan);
                }
                else
                {
                    SymbolTable.Rows.Add(lex2, scantype, scan);
                }
            }
            else if (tok == "OPERATOR_MATH_ADD" || tok == "OPERATOR_MATH_SUB" ||
                tok == "OPERATOR_MATH_MUL" || tok == "OPERATOR_MATH_DIV" || tok == "OPERATOR_MATH_MOD")
            {

                Stack st1 = new Stack();
                Stack st2 = new Stack();

                st1.Push(tok);//push operator

                //Console.Write(st1.Pop());
                //st1.Push(tok);
                while (st1.Count > 0)
                {
                    index++;//operator1
                    string lex2 = (string)LexTable.Rows[index].Cells[0].Value;
                    string tok2 = (string)LexTable.Rows[index].Cells[1].Value;
                    //index++;
                    //Console.Write(tok2);

                    if (tok2 == "NUMBR_LITERAL" || tok2 == "NUMBAR_LITERAL")
                    {
                        st2.Push(lex2);
                    }
                    else if (tok2 == "VARIABLE_NAME")
                    {
                        st2.Push(lex2);
                    }
                    else if (Regex.IsMatch(tok2, @"OPERATOR_MATH_ADD") || Regex.IsMatch(tok2, @"OPERATOR_MATH_SUB") ||
                        Regex.IsMatch(tok2, @"OPERATOR_MATH_MUL") || Regex.IsMatch(tok2, @"OPERATOR_MATH_DIV") || Regex.IsMatch(tok2, @"OPERATOR_MATH_MOD") || tok == "OPERATOR_MATH_MAX" || tok == "OPERATOR_MATH_MIN")
                    {
                        st1.Push(tok2);//push operator
                    }

                    Double ans;
                    if (st2.Count >= 2)
                    {
                        string operand = (string)st1.Pop();
                        string operand1 = (string)st2.Pop();
                        string operand2 = (string)st2.Pop();

                        /*int rowIndex2 = -1;
                        foreach (DataGridViewRow row in SymbolTable.Rows)
                        {
                            if (operand1 == (string)row.Cells[0].Value)
                            {
                                rowIndex2 = row.Index;
                                break;
                            }
                        }
                        if (rowIndex2 > -1)
                            operand1 = (string)SymbolTable.Rows[index].Cells[2].Value;
                        else
                            operand1 = "0";

                        rowIndex2 = -1;
                        foreach (DataGridViewRow row in SymbolTable.Rows)
                        {
                            if (operand2 == (string)row.Cells[0].Value)
                            {
                                rowIndex2 = row.Index;
                                break;
                            }
                        }
                        if (rowIndex2 > -1)
                            operand2 = (string)SymbolTable.Rows[index].Cells[2].Value;
                        else
                            operand2 = "0";
                        */

                        if (Regex.IsMatch(operand, @"ADD"))
                        {
                            ans = Convert.ToDouble(operand1) + Convert.ToDouble(operand2);
                            st2.Push(Convert.ToString(ans));
                        }
                        else if (Regex.IsMatch(operand, @"SUB"))
                        {
                            ans = Convert.ToDouble(operand2) - Convert.ToDouble(operand1);
                            st2.Push(Convert.ToString(ans));
                        }
                        else if (Regex.IsMatch(operand, @"MUL"))
                        {
                            ans = Convert.ToDouble(operand1) * Convert.ToDouble(operand2);
                            st2.Push(Convert.ToString(ans));
                        }
                        else if (Regex.IsMatch(operand, @"DIV"))
                        {
                            ans = Convert.ToDouble(operand2) / Convert.ToDouble(operand1);
                            st2.Push(Convert.ToString(ans));
                        }
                        else if (Regex.IsMatch(operand, @"MOD"))
                        {
                            ans = Convert.ToDouble(operand1) % Convert.ToDouble(operand2);
                            st2.Push(Convert.ToString(ans));
                        }
                        else if (Regex.IsMatch(operand, @"MAX"))
                        {
                            if (Convert.ToDouble(operand1) > Convert.ToDouble(operand2))
                                ans = Convert.ToDouble(operand1);
                            else
                                ans = Convert.ToDouble(operand2);

                            st2.Push(Convert.ToString(ans));
                        }
                        else if (Regex.IsMatch(operand, @"MIN"))
                        {
                            if (Convert.ToDouble(operand1) < Convert.ToDouble(operand2))
                                ans = Convert.ToDouble(operand1);
                            else
                                ans = Convert.ToDouble(operand2);

                            st2.Push(Convert.ToString(ans));
                        }
                    }

                }
                String searchValue = "IT";
                int rowIndex = -1;
                foreach (DataGridViewRow row in SymbolTable.Rows)
                {
                    if (row.Cells[0].Value.ToString().Equals(searchValue))
                    {
                        rowIndex = row.Index;
                        break;
                    }
                }
                SymbolTable.Rows.Remove(SymbolTable.Rows[rowIndex]);

                SymbolTable.Rows.Add("IT", "NUMBR_LITERAL", Convert.ToString(st2.Pop()));

            }
            else if (tok == "OPERATORS_BOOLEAN_AND")
            {
                index++;//var name/first operand
                string lex2 = LexTable.Rows[index].Cells[0].Value.ToString();
                string tok2 = LexTable.Rows[index].Cells[1].Value.ToString();

                string operand1 = "0";
                string operand2 = "0";

                if (tok2 == "VARIABLE_NAME")
                {
                    int rowIndex2 = -1;
                    foreach (DataGridViewRow row in SymbolTable.Rows)
                    {
                        if (lex2 == (string)row.Cells[0].Value )
                        {
                            rowIndex2 = row.Index;
                            break;
                        }
                    }
                    if (rowIndex2 > -1)
                        operand1 = (string)SymbolTable.Rows[index].Cells[2].Value;
                    else
                        operand1 = "0";
                }
                index++;//an
                index++;//second operand
                 lex2 = LexTable.Rows[index].Cells[0].Value.ToString();
                 tok2 = LexTable.Rows[index].Cells[1].Value.ToString();

                 if (tok2 == "VARIABLE_NAME")
                 {
                     int rowIndex2 = -1;
                     foreach (DataGridViewRow row in SymbolTable.Rows)
                     {
                         if (lex2 == (string)row.Cells[0].Value)
                         {
                             rowIndex2 = row.Index;
                             break;
                         }
                     }
                     if (rowIndex2 > -1)
                         operand2 = (string)SymbolTable.Rows[index].Cells[2].Value;
                     else
                         operand2 = "0";
                 }

                 if (Convert.ToDouble(operand1) != 0 && Convert.ToDouble(operand2) != 0)
                 {
                     int rowIndex = -1;
                     foreach (DataGridViewRow row in SymbolTable.Rows)
                     {
                         string str = (string)row.Cells[0].Value;
                         if (str == "IT")
                         {
                             rowIndex = row.Index;
                             break;
                         }
                     }
                     SymbolTable.Rows.Remove(SymbolTable.Rows[rowIndex]);

                     SymbolTable.Rows.Add("IT", "TROOF", "WIN");
                 }
                 else
                 {
                     int rowIndex = -1;
                     foreach (DataGridViewRow row in SymbolTable.Rows)
                     {
                         string str = (string)row.Cells[0].Value;
                         if (str == "IT")
                         {
                             rowIndex = row.Index;
                             break;
                         }
                     }
                     SymbolTable.Rows.Remove(SymbolTable.Rows[rowIndex]);

                     SymbolTable.Rows.Add("IT", "TROOF", "FAIL");
                 }
            }
        }

        private Boolean checkOperatorsComparison(string str)
        {
            int y = 0;
            String tempVar = System.String.Empty;

            if (Regex.IsMatch(str, @"^BOTH SAEM") || Regex.IsMatch(str, @"^DIFFRINT"))
            {
                if (Regex.IsMatch(str, @"^BOTH SAEM")) {
                    LexTable.Rows.Add("BOTH SAEM", "OPERATORS_COMPARISON_EQUAL");
                    str = str.Substring(9);
                    str = str.Trim();
                }
                else if (Regex.IsMatch(str, @"^DIFFRINT"))
                {
                    LexTable.Rows.Add("DIFFRINT", "OPERATORS_COMPARISON_NOT_EQUAL");
                    str = str.Substring(9);
                    str = str.Trim();
                }

                // extract numbr or numbar, variable
                // this gets the whitespace index
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == ' ')
                    {
                        y = i;
                        break;
                    }
                }

                // extracts numbr, numbar
                tempVar = str.Substring(0, y);

                // removes 1st operand
                str = str.Substring(y);
                str = str.Trim();

                if (Regex.IsMatch(tempVar, @"^-?[0-9]+.[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBAR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^-?[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^WIN$"))
                    LexTable.Rows.Add(tempVar, "BOOLEAN_TRUE");
                else if (Regex.IsMatch(tempVar, @"^FAIL$"))
                    LexTable.Rows.Add(tempVar, "BOOLEAN_FALSE");
                else if (Regex.IsMatch(tempVar, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                    LexTable.Rows.Add(tempVar, "VARIABLE_NAME");
                else return true;

                // AND
                if (Regex.IsMatch(str, "^AND"))
                {
                    if (Regex.IsMatch(str, @"^AND BIGGR OF")) {
                        LexTable.Rows.Add("AND BIGGR OF", "OPERATORS_COMPARISON_GREATER_THAN");
                        str = str.Substring(12);
                        str = str.Trim();
                    }
                    else if (Regex.IsMatch(str, @"^AND SMALLR OF")) {
                        LexTable.Rows.Add("AND SMALLR OF", "OPERATORS_COMPARISON_LESSER_THAN");
                        str = str.Substring(13);
                        str = str.Trim();
                    }

                    // extract numbr or numbar, variable
                    // this gets the whitespace index
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (str[i] == ' ')
                        {
                            y = i;
                            break;
                        }
                    }

                    // extracts numbr, numbar
                    tempVar = str.Substring(0, y);

                    // removes 1st operand
                    str = str.Substring(y);
                    str = str.Trim();

                    if (Regex.IsMatch(tempVar, @"^-?[0-9]+.[0-9]+"))
                        LexTable.Rows.Add(tempVar, "NUMBAR_LITERAL");
                    else if (Regex.IsMatch(tempVar, @"^-?[0-9]+"))
                        LexTable.Rows.Add(tempVar, "NUMBR_LITERAL");
                    else if (Regex.IsMatch(tempVar, @"^WIN$"))
                        LexTable.Rows.Add(tempVar, "BOOLEAN_TRUE");
                    else if (Regex.IsMatch(tempVar, @"^FAIL$"))
                        LexTable.Rows.Add(tempVar, "BOOLEAN_FALSE");
                    else if (Regex.IsMatch(tempVar, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                        LexTable.Rows.Add(tempVar, "VARIABLE_NAME");
                    else return true;

                    // checks <AN> binary operator
                    if (Regex.IsMatch(str, @"^AN"))
                    {
                        LexTable.Rows.Add("AN", "INFIX_BINARY");

                        // removes AN operator
                        str = str.Substring(2);
                        str = str.Trim();
                    }
                    else return true;

                    // extract numbr or numbar, variable
                    // this gets the whitespace index
                    y = 0;
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (str[i] == ' ')
                        {
                            y = i;
                            break;
                        }
                    }

                    if (y != 0)
                    {
                        // extracts numbr, numbar
                        tempVar = str.Substring(0, y);

                        // removes 2nd operand
                        str = str.Substring(y);
                        str = str.Trim();
                    }
                    else tempVar = str;

                    if (Regex.IsMatch(tempVar, @"^-?[0-9]+.[0-9]+"))
                        LexTable.Rows.Add(tempVar, "NUMBAR_LITERAL");
                    else if (Regex.IsMatch(tempVar, @"^-?[0-9]+"))
                        LexTable.Rows.Add(tempVar, "NUMBR_LITERAL");
                    else if (Regex.IsMatch(tempVar, @"^WIN$"))
                        LexTable.Rows.Add(tempVar, "BOOLEAN_TRUE");
                    else if (Regex.IsMatch(tempVar, @"^FAIL$"))
                        LexTable.Rows.Add(tempVar, "BOOLEAN_FALSE");
                    else if (Regex.IsMatch(tempVar, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                        LexTable.Rows.Add(tempVar, "VARIABLE_NAME");
                    else return true;

                    return false;
                }

                // AN
                else if (Regex.IsMatch(str, "^AN"))
                {
                    LexTable.Rows.Add("AN", "INFIX_BINARY");

                    // removes AN operator
                    str = str.Substring(2);
                    str = str.Trim();


                    // extract numbr or numbar, variable
                    // this gets the whitespace index
                    y = 0;
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (str[i] == ' ')
                        {
                            y = i;
                            break;
                        }
                    }

                    if (y != 0)
                    {
                        // extracts numbr, numbar
                        tempVar = str.Substring(0, y);

                        // removes 2nd operand
                        str = str.Substring(y);
                        str = str.Trim();
                    }
                    else tempVar = str;

                    if (Regex.IsMatch(tempVar, @"^-?[0-9]+.[0-9]+"))
                        LexTable.Rows.Add(tempVar, "NUMBAR_LITERAL");
                    else if (Regex.IsMatch(tempVar, @"^-?[0-9]+"))
                        LexTable.Rows.Add(tempVar, "NUMBR_LITERAL");
                    else if (Regex.IsMatch(tempVar, @"^WIN$"))
                        LexTable.Rows.Add(tempVar, "BOOLEAN_TRUE");
                    else if (Regex.IsMatch(tempVar, @"^FAIL$"))
                        LexTable.Rows.Add(tempVar, "BOOLEAN_FALSE");
                    else if (Regex.IsMatch(tempVar, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                        LexTable.Rows.Add(tempVar, "VARIABLE_NAME");
                    else return true;

                    return false;
                }
                else return true;

            }

            return true;
        }


        private Boolean checkOperatorsBoolean(String str)
        {
            int y = 0;
            String tempVar = System.String.Empty;
            // Binary operators
            if (Regex.IsMatch(str, @"^BOTH OF") || Regex.IsMatch(str, @"^EITHER OF") || Regex.IsMatch(str, @"^WON OF"))
            {
                
                if (Regex.IsMatch(str, @"^BOTH OF")) {
                    LexTable.Rows.Add("BOTH OF", "OPERATORS_BOOLEAN_AND");
                    str = str.Substring(7);
                    str = str.Trim();
                }
                else if (Regex.IsMatch(str, @"^EITHER OF"))
                {
                    LexTable.Rows.Add("EITHER OF", "OPERATORS_BOOLEAN_OR");
                    str = str.Substring(9);
                    str = str.Trim();
                }
                else if (Regex.IsMatch(str, @"^WON OF"))
                {
                    LexTable.Rows.Add("WON OF", "OPERATORS_BOOLEAN_XOR");
                    str = str.Substring(6);
                    str = str.Trim();
                }

                // extract numbr or numbar, variable
                // this gets the whitespace index
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == ' ')
                    {
                        y = i;
                        break;
                    }
                }

                // extracts numbr, numbar
                tempVar = str.Substring(0, y);

                // removes 1st operand
                str = str.Substring(y);
                str = str.Trim();

                if (Regex.IsMatch(tempVar, @"^-?[0-9]+.[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBAR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^-?[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^WIN$"))
                    LexTable.Rows.Add(tempVar, "BOOLEAN_TRUE");
                else if (Regex.IsMatch(tempVar, @"^FAIL$"))
                    LexTable.Rows.Add(tempVar, "BOOLEAN_FALSE");
                else if (Regex.IsMatch(tempVar, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                    LexTable.Rows.Add(tempVar, "VARIABLE_NAME");
                else return true;

                // checks <AN> binary operator
                if (Regex.IsMatch(str, @"^AN"))
                {
                    LexTable.Rows.Add("AN", "INFIX_BINARY");

                    // removes AN operator
                    str = str.Substring(2);
                    str = str.Trim();
                }
                else return true;

                // extract numbr or numbar, variable
                // this gets the whitespace index
                y = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == ' ')
                    {
                        y = i;
                        break;
                    }
                }

                if (y != 0)
                {
                    // extracts numbr, numbar
                    tempVar = str.Substring(0, y);

                    // removes 2nd operand
                    str = str.Substring(y);
                    str = str.Trim();
                }
                else tempVar = str;

                if (Regex.IsMatch(tempVar, @"^-?[0-9]+.[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBAR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^-?[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^WIN$"))
                    LexTable.Rows.Add(tempVar, "BOOLEAN_TRUE");
                else if (Regex.IsMatch(tempVar, @"^FAIL$"))
                    LexTable.Rows.Add(tempVar, "BOOLEAN_FALSE");
                else if (Regex.IsMatch(tempVar, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                    LexTable.Rows.Add(tempVar, "VARIABLE_NAME");
                else return true;

                return false;
            }

            // unary operators
            else if (Regex.IsMatch(str, @"^NOT OF")) {
                LexTable.Rows.Add("NOT OF", "OPERATORS_BOOLEAN_NOT");
                str = str.Substring(6);
                str = str.Trim();

                if (Regex.IsMatch(str, @"^-?[0-9]+.[0-9]+"))
                    LexTable.Rows.Add(str, "NUMBAR_LITERAL");
                else if (Regex.IsMatch(str, @"^-?[0-9]+"))
                    LexTable.Rows.Add(str, "NUMBR_LITERAL");
                else if (Regex.IsMatch(str, @"^WIN$"))
                    LexTable.Rows.Add(str, "BOOLEAN_TRUE");
                else if (Regex.IsMatch(str, @"^FAIL$"))
                    LexTable.Rows.Add(str, "BOOLEAN_FALSE");
                else if (Regex.IsMatch(str, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                    LexTable.Rows.Add(str, "VARIABLE_NAME");
                else return true;

                return false;
            }

            // n-ary operators
            else if (Regex.IsMatch(str, @"^ALL OF") || Regex.IsMatch(str, @"^ANY OF"))
            {
                if (Regex.IsMatch(str, @"^ALL OF"))
                {
                    LexTable.Rows.Add("ALL OF", "OPERATORS_BOOLEAN_AND_INFINITE");
                    str = str.Substring(6);
                    str = str.Trim();
                }
                else if (Regex.IsMatch(str, @"^ANY OF"))
                {
                    LexTable.Rows.Add("ANY OF", "OPERATORS_BOOLEAN_OR_INFINITE");
                    str = str.Substring(6);
                    str = str.Trim();
                }

                // extract numbr or numbar, variable
                // this gets the whitespace index
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == ' ')
                    {
                        y = i;
                        break;
                    }
                }

                
                // extracts numbr, numbar
                tempVar = str.Substring(0, y);

                // removes 2nd operand
                str = str.Substring(y);
                str = str.Trim();

                if (Regex.IsMatch(tempVar, @"^-?[0-9]+.[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBAR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^-?[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^WIN$"))
                    LexTable.Rows.Add(tempVar, "BOOLEAN_TRUE");
                else if (Regex.IsMatch(tempVar, @"^FAIL$"))
                    LexTable.Rows.Add(tempVar, "BOOLEAN_FALSE");
                else if (Regex.IsMatch(tempVar, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                    LexTable.Rows.Add(tempVar, "VARIABLE_NAME");
                else return true;

                // checks <AN> binary operator
                if (Regex.IsMatch(str, @"^AN"))
                {
                    LexTable.Rows.Add("AN", "INFIX_BINARY");

                    // removes AN operator
                    str = str.Substring(2);
                    str = str.Trim();
                }
                else return true;

                // extract numbr or numbar, variable
                // this gets the whitespace index
                y = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == ' ')
                    {
                        y = i;
                        break;
                    }
                }

                if (y != 0)
                {
                    // extracts numbr, numbar
                    tempVar = str.Substring(0, y);

                    // removes 2nd operand
                    str = str.Substring(y);
                    str = str.Trim();
                }
                else tempVar = str;

                if (Regex.IsMatch(tempVar, @"^-?[0-9]+.[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBAR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^-?[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^WIN$"))
                    LexTable.Rows.Add(tempVar, "BOOLEAN_TRUE");
                else if (Regex.IsMatch(tempVar, @"^FAIL$"))
                    LexTable.Rows.Add(tempVar, "BOOLEAN_FALSE");
                else if (Regex.IsMatch(tempVar, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                    LexTable.Rows.Add(tempVar, "VARIABLE_NAME");
                else return true;

                while (y != 0) {

                    if (Regex.IsMatch(str, @"^MKAY$"))
                    {
                        LexTable.Rows.Add("MKAY", "ARITY_CLOSE");
                        return false;
                    }

                    // checks <AN> binary operator
                    if (Regex.IsMatch(str, @"^AN"))
                    {
                        LexTable.Rows.Add("AN", "INFIX_BINARY");

                        // removes AN operator
                        str = str.Substring(2);
                        str = str.Trim();
                    }
                    else return true;

                    // extract numbr or numbar, variable
                    // this gets the whitespace index
                    y = 0;
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (str[i] == ' ')
                        {
                            y = i;
                            break;
                        }
                    }

                    if (y != 0)
                    {
                        // extracts numbr, numbar
                        tempVar = str.Substring(0, y);

                        // removes 2nd operand
                        str = str.Substring(y);
                        str = str.Trim();
                    }
                    else tempVar = str;

                    if (Regex.IsMatch(tempVar, @"^-?[0-9]+.[0-9]+"))
                        LexTable.Rows.Add(tempVar, "NUMBAR_LITERAL");
                    else if (Regex.IsMatch(tempVar, @"^-?[0-9]+"))
                        LexTable.Rows.Add(tempVar, "NUMBR_LITERAL");
                    else if (Regex.IsMatch(tempVar, @"^WIN$"))
                        LexTable.Rows.Add(tempVar, "BOOLEAN_TRUE");
                    else if (Regex.IsMatch(tempVar, @"^FAIL$"))
                        LexTable.Rows.Add(tempVar, "BOOLEAN_FALSE");
                    else if (Regex.IsMatch(tempVar, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                        LexTable.Rows.Add(tempVar, "VARIABLE_NAME");
                    else return true;
                }


                return false;
            }

            return true;
        }


        private Boolean checkOperatorsMath(String str)
        {
            Boolean errorFlag = false;
            Boolean firstOperandFlag = false;
            List<int> indexes;
            int y = 0;
            String tempVar = System.String.Empty;

            if (Regex.IsMatch(str, @"^SUM OF")) {

                LexTable.Rows.Add("SUM OF", "OPERATOR_MATH_ADD");
                str = str.Substring(6);
                str = str.Trim();
            }
            else if (Regex.IsMatch(str, @"^DIFF OF"))
            {

                LexTable.Rows.Add("DIFF OF", "OPERATOR_MATH_SUB");
                str = str.Substring(7);
                str = str.Trim();
            }
            else if (Regex.IsMatch(str, @"^PRODUKT OF"))
            {

                LexTable.Rows.Add("PRODUKT OF", "OPERATOR_MATH_MUL");
                str = str.Substring(10);
                str = str.Trim();
            }
            else if (Regex.IsMatch(str, @"^QUOSHUNT OF"))
            {

                LexTable.Rows.Add("QUOSHUNT OF", "OPERATOR_MATH_DIV");
                str = str.Substring(11);
                str = str.Trim();
            }
            else if (Regex.IsMatch(str, @"^MOD OF"))
            {

                LexTable.Rows.Add("MOD OF", "OPERATOR_MATH_MOD");
                str = str.Substring(6);
                str = str.Trim();
            }
            else if (Regex.IsMatch(str, @"^BIGGR OF"))
            {

                LexTable.Rows.Add("BIGGR OF", "OPERATOR_MATH_MAX");
                str = str.Substring(8);
                str = str.Trim();
            }
            else if (Regex.IsMatch(str, @"^SMALLR OF"))
            {

                LexTable.Rows.Add("SMALLR OF", "OPERATOR_MATH_MIN");
                str = str.Substring(9);
                str = str.Trim();
            }


            /* checks the first operand if a variable_name, number, or expression */
            if (Regex.IsMatch(str, @"^SUM OF") || Regex.IsMatch(str, @"^DIFF OF") ||
                Regex.IsMatch(str, @"^PRODUKT OF") || Regex.IsMatch(str, @"^QUOSHUNT OF") ||
                Regex.IsMatch(str, @"^MOD OF") || Regex.IsMatch(str, @"^BIGGR OF") ||
                Regex.IsMatch(str, @"^SMALLR OF"))
            {
                errorFlag = checkOperatorsMath(str);
                firstOperandFlag = true;
                if (errorFlag == true) return true;
            }

            if (firstOperandFlag == false)
            {

                // if not an expression, extract numbr or numbar
                // this gets the whitespace index
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == ' ')
                    {
                        y = i;
                        break;
                    }
                }

                // extracts numbr, numbar
                tempVar = str.Substring(0, y);

                // removes 1st operand
                str = str.Substring(y);
                str = str.Trim();

                if (Regex.IsMatch(tempVar, @"^-?[0-9]+.[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBAR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^-?[0-9]+"))
                    LexTable.Rows.Add(tempVar, "NUMBR_LITERAL");
                else if (Regex.IsMatch(tempVar, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                    LexTable.Rows.Add(tempVar, "VARIABLE_NAME");
            }
            else
            {
                indexes = findIndexSubstring(str, "AN");
                str = str.Substring(indexes[1]);
                str = str.Trim();
            }


            // checks <AN> binary operator
            if (Regex.IsMatch(str, @"^AN")) {
                LexTable.Rows.Add("AN", "INFIX_BINARY");

                // removes AN operator
                str = str.Substring(2);
                str = str.Trim();
            }
            else return true;



            /* checks the second operand if a variable_name, number, or expression */
            if (Regex.IsMatch(str, @"^SUM OF") || Regex.IsMatch(str, @"^DIFF OF") ||
                Regex.IsMatch(str, @"^PRODUKT OF") || Regex.IsMatch(str, @"^QUOSHUNT OF") ||
                Regex.IsMatch(str, @"^MOD OF") || Regex.IsMatch(str, @"^BIGGR OF") ||
                Regex.IsMatch(str, @"^SMALLR OF"))
            {
                errorFlag = checkOperatorsMath(str);
                if (errorFlag == true) return true;
                else return false;
            }


            // if not an expression, extract numbr or numbar
            // this gets the whitespace index
            y = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == ' ')
                {
                    y = i;
                    break;
                }
            }

            if (y != 0)
            {
                // extracts numbr, numbar
                tempVar = str.Substring(0, y);

                // removes 2nd operand
                str = str.Substring(y);
                str = str.Trim();
            }
            else tempVar = str;


            if (Regex.IsMatch(tempVar, @"^-?[0-9]+.[0-9]+"))
                LexTable.Rows.Add(tempVar, "NUMBAR_LITERAL");
            else if (Regex.IsMatch(tempVar, @"^-?[0-9]+"))
                LexTable.Rows.Add(tempVar, "NUMBR_LITERAL");
            else if (Regex.IsMatch(tempVar, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                LexTable.Rows.Add(tempVar, "VARIABLE_NAME");


            if (errorFlag == true)
                return true;
            




            return false;
        } // end of checkOperatorsMath()



        private Boolean variable_assignment(List<String> toks) {

            int x = 0, y = 0;
            String temp = System.String.Empty;
            String tempVar = System.String.Empty;

            if (Regex.IsMatch(toks[0], @"^[a-zA-Z][a-zA-Z0-9_]*\s+R.*")) {

                // finds space between <var_name> R
                temp = toks[0];
                for (int i = 0; i < toks[0].Length - 1; i++) {
                    if (temp[i] == ' ') {
                        y = i;
                        break;
                    }
                }

                // copy the string var to tempvar
                tempVar = temp.Substring(0, y);
                LexTable.Rows.Add(tempVar, "VARIABLE_NAME");

                // remove Variable_name
                temp = temp.Substring(y);
                temp = temp.Trim();

                LexTable.Rows.Add("R", "VARIABLE_VALUE_ASSIGNMENT");

                // remove R keyword
                temp = temp.Substring(2);
                temp = temp.Trim();

                if (temp.Length == 0)
                    return true;

                // value assignment is a YARN
                if (temp[0] == '\"')
                {
                    if (temp[temp.Length - 1] == '\"')
                    {
                        LexTable.Rows.Add(temp, "YARN_LITERAL");
                        return false;
                    }
                    else return true;
                }
                // checks if a NUMBAR
                else if (Regex.IsMatch(temp, @"^-?[0-9]+.[0-9]+"))
                {
                    LexTable.Rows.Add(temp, "NUMBAR_LITERAL");
                    return false;
                }
                else if (Regex.IsMatch(temp, @"^-?[0-9]+"))
                {
                    LexTable.Rows.Add(temp, "NUMBR_LITERAL");
                    return false;
                }
                else if (Regex.IsMatch(temp, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                {
                    LexTable.Rows.Add(temp, "VARIABLE_NAME");
                    return false;
                }
                else if (Regex.IsMatch(temp, @"^SUM OF") || Regex.IsMatch(temp, @"^DIFF OF") ||
                         Regex.IsMatch(temp, @"^PRODUKT OF") || Regex.IsMatch(temp, @"^QUOSHUNT OF") ||
                         Regex.IsMatch(temp, @"^MOD OF") || Regex.IsMatch(temp, @"^BIGGR OF") ||
                         Regex.IsMatch(temp, @"^SMALLR OF"))
                    return checkOperatorsMath(temp);
                else if (Regex.IsMatch(temp, @"^BOTH OF") || Regex.IsMatch(temp, @"^EITHER OF") ||
                             Regex.IsMatch(temp, @"^WON OF") || Regex.IsMatch(temp, @"^NOT OF") ||
                             Regex.IsMatch(temp, @"^ALL OF") || Regex.IsMatch(temp, @"^ANY OF"))
                    return checkOperatorsBoolean(temp);
            }

            return true;
        } // end of variable_assignment()

        

        private Boolean variable_declaration(List<String> toks) {

            int x = 0, y = 0;
            String varTemp = System.String.Empty;

            if (Regex.IsMatch(toks[0], @"^I HAS A")) {

                // adds I HAS A keyword to the lex table
                LexTable.Rows.Add("I HAS A", "VARIABLE_DECLARATION");

                // removes I HAS A keyword in line
                String temp = toks[0].Substring(8);

                // removes trailing whitespaces
                temp = temp.Trim();

                // variable name is matched
                if (Regex.IsMatch(temp, @"^[a-zA-Z][a-zA-Z0-9_]*\s*.*$"))
                {

                    // ITZ keyword found
                    if (Regex.IsMatch(temp, @"ITZ"))
                    {
                        // localize the variable name token
                        for (int i = 0; i < temp.Length; i++)
                        {
                            if (temp[i] == ' ')
                            {
                                y = i;
                                break;
                            }
                        }

                        // copy the string var to tempvar
                        varTemp = temp.Substring(0, y);
                        LexTable.Rows.Add(varTemp, "VARIABLE_NAME");

                        // remove Variable_name
                        temp = temp.Substring(y);
                        temp = temp.Trim();

                        LexTable.Rows.Add("ITZ", "VARIABLE_DECLARATION_VALUE_ASSIGNMENT");

                        // remove ITZ keyword
                        temp = temp.Substring(4);
                        temp = temp.Trim();

                        if (temp.Length == 0)
                            return true;

                        // value assignment is a YARN
                        if (temp[0] == '\"')
                        {
                            if (temp[temp.Length - 1] == '\"')
                            {
                                LexTable.Rows.Add(temp, "YARN_LITERAL");
                                return false;
                            }
                            else return true;
                        }
                        // checks if a NUMBAR
                        else if (Regex.IsMatch(temp, @"^-?[0-9]+.[0-9]+"))
                        {
                            LexTable.Rows.Add(temp, "NUMBAR_LITERAL");
                            return false;
                        }
                        else if (Regex.IsMatch(temp, @"^-?[0-9]+"))
                        {
                            LexTable.Rows.Add(temp, "NUMBR_LITERAL");
                            return false;
                        }
                        else if (Regex.IsMatch(temp, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                        {
                            LexTable.Rows.Add(temp, "VARIABLE_NAME");
                            return false;
                        }
                        else if (Regex.IsMatch(temp, @"^SUM OF") || Regex.IsMatch(temp, @"^DIFF OF") ||
                                 Regex.IsMatch(temp, @"^PRODUKT OF") || Regex.IsMatch(temp, @"^QUOSHUNT OF") ||
                                 Regex.IsMatch(temp, @"^MOD OF") || Regex.IsMatch(temp, @"^BIGGR OF") ||
                                 Regex.IsMatch(temp, @"^SMALLR OF"))
                            return checkOperatorsMath(temp);
                        else if (Regex.IsMatch(temp, @"^BOTH OF") || Regex.IsMatch(temp, @"^EITHER OF") ||
                                 Regex.IsMatch(temp, @"^WON OF") || Regex.IsMatch(temp, @"^NOT OF") ||
                                 Regex.IsMatch(temp, @"^ALL OF") || Regex.IsMatch(temp, @"^ANY OF"))
                            return checkOperatorsBoolean(temp);


                        return false;
                    }

                    // no ITZ keyword
                    else {

                        if (Regex.IsMatch(temp, @"\s+"))
                            return true;

                        LexTable.Rows.Add(temp, "VARIABLE_NAME");
                        return false;
                    }

                }
                else return true;

            }


            return true;
        } // end of variable_declaration()


        private Boolean checkIO_scan(List<String> toks) {

            

            if (Regex.IsMatch(toks[0], @"^GIMMEH")) {
            
                // adds GIMMEH keyword to the lex table
                LexTable.Rows.Add("GIMMEH", "IO_SCAN");

                // removes GIMMEH keyword in line
                String temp = toks[0].Substring(7);

                // removes trailing whitespaces
                temp = temp.Trim();

                // check if variable name has invalid characters
                if (Regex.IsMatch(temp, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                {
                    LexTable.Rows.Add(temp, "VARIABLE_NAME");
                    return false;
                    
                }

                return true;
            }

            return true;
        } // end of checkIO_scan()


        private Boolean checkIO_print(List<String> toks) {

            Boolean yarnFlag = false;
            String temp = System.String.Empty;
            int x = 0, y = 0;

            

            if (Regex.IsMatch(toks[0], @"^VISIBLE")) {

                // adds VISIBLE keyword to the lex table
                LexTable.Rows.Add("VISIBLE", "IO_PRINT");

                // removes VISIBLE keyword in line
                toks[0] = toks[0].Substring(8);

                // removes trailing whitespaces
                temp = temp.Trim();

                // check if variable name has invalid characters
                if (Regex.IsMatch(temp, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                {
                    LexTable.Rows.Add(temp, "VARIABLE_NAME");
                    return false;
                }


                // checks if the operand is a YARN by finding opening " mark
                temp = toks[0];
                for (int i=0; i<temp.Length; i++) {
                    if (temp[i] == '\"') {
                        yarnFlag = true;
                        x = i;
                        break;
                    }
                }
                
                // for YARN operand
                if (yarnFlag == true)
                {

                    // finds ending " mark
                    for (int i = x + 1; i < temp.Length; i++)
                    {
                        if (temp[i] == '\"')
                        {
                            y = i;
                            break;
                        }
                    }

                    // gets the YARN substring
                    temp = toks[0].Substring(x, y - x +1 );

                    // adds yarn literal to lex table
                    LexTable.Rows.Add(temp, "YARN_LITERAL");


                    return false;
                }

                // for Variable operand
                else
                {
                    // removes trailing whitespaces
                    temp = temp.Trim();

                    // check if variable name has invalid characters
                    if (Regex.IsMatch(temp, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                    {
                        LexTable.Rows.Add(temp, "VARIABLE_NAME");
                        return false;
                    }

                    return true;
                }

                
            }

            return true;
        }   //end of checkIO_print()


        /* retrieves text from a selected text file and loads it in the RTB */
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            openCode.Title = "Select LOL code";

            // sets filters for file types
            openCode.Filter = "LOL Code|*.lol|All files|*.*";

            // opens the dialog box for selecting files
            if (openCode.ShowDialog() != DialogResult.Cancel)
            {
                string selected_file = openCode.FileName;

                // clears the RTB
                boxCode.SelectAll();
                boxCode.Cut();

                boxCode.Text = File.ReadAllText(selected_file);
            }



        }

        private void boxCode_TextChanged(object sender, EventArgs e) {

 
        }

        private string[] splitter(String full){
            return Regex.Split(full, "\n");
        }

        private void boxCode_TextChanged_1(object sender, EventArgs e) {

        }

        private List<int> findIndexSubstring(string str, string value) {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }


    }


}
