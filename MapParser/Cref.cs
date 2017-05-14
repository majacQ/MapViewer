﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapViewer
{
    // A cross reference table entry is of the form:
    // SymbolName ....      ModuleName (where the symbol was defined in)
    //                      User Module 1
    //                      User Module 2 ..
    class CrefEntry
    {
        public string SymbolName;
        public string SouceModule;
        public List<string> Users;

        public CrefEntry()
        {
            Users = new List<string>();
        }

        public override string ToString()
        {
            string s = "Cref: " + SymbolName + " " + SouceModule + "\n";
            foreach(string l in Users) s += l + "\n";

            return s;
        }
    }

    class Cref
    {
        bool DEBUG = true;
        string CrefHeader = "Cross Reference Table";
        List<CrefEntry> CrefTable;

        public Cref()
        {
            CrefTable = new List<CrefEntry>();
        }

        public bool Build(string mapFile)
        {
            List<string> Map = File.ReadAllLines(mapFile).ToList();
            int Cref_index = Map.FindIndex(x => String.Equals(CrefHeader, x));
            int Map_end = Map.Count;
            if ((Cref_index == -1) || (Map_end == -1))
            {
                MessageBox.Show("Couldn't find Cross Reference Table in map file! Can't proceed!", "Oops!", MessageBoxButtons.OK); return false;
            }
            Debug.WriteLineIf(DEBUG, "Found Cref at index :" + Cref_index.ToString());

            Cref_index += 3; // skip over the first 3 lines
            foreach (string line in Map.GetRange(Cref_index, Map_end - Cref_index))
            {
                if (line == "") break; // exit on encountering an empty line. Presumably the Cref section has ended

                Debug.WriteLineIf(DEBUG, CrefTable.LastOrDefault()?.ToString());
                if (line[0] == ' ')
                    CrefTable.LastOrDefault()?.Users.Add(line.TrimStart());
                else
                    AddNewCref(line);
            }
            return true;
        }

        bool AddNewCref(string line)
        {
            string symName = "";
            string modPath = "";
            string[] ele = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(ele.Length >= 2);

            symName = ele[0];
            modPath = line.Substring(ele[0].Length).TrimStart();

            CrefTable.Add(new CrefEntry { SymbolName = symName, SouceModule = modPath });

            return true;
        }
    }
}