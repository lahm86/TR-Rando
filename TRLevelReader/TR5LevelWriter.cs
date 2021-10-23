﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRLevelReader.Model;

namespace TRLevelReader
{
    public class TR5LevelWriter
    {
        public TR5LevelWriter()
        {

        }

        public void WriteLevelToFile(TR5Level lvl, string filepath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create(filepath)))
            {
                writer.Write(lvl.Serialize());
            }
        }
    }
}