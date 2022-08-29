﻿using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRLevelReader.Model;
using TRLevelToolset.Interfaces;
using TRLevelToolset.IOLogic;

namespace TRLevelToolset.Controls.DataControls.TR
{
    internal class TRDemoDataControl : IDrawable
    {
        public void Draw()
        {
            if (ImGui.TreeNodeEx("Demo Data", ImGuiTreeNodeFlags.OpenOnArrow))
            {
                ImGui.Text("Demo Data Size: " + IOManager.CurrentLevelAsTR1?.NumDemoData);
                ImGui.TreePop();
            }
        }
    }
}