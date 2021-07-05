using System;
using System.Collections.Generic;
using CyrusVance.CoolQRobot.Messages.CQElements.Base;
namespace CyrusVance.CoolQRobot.Messages.CQElements {
    /// <summary></summary>
    [Obsolete ("可以直接使用ElementFace(id,\"emoji\")", false)]
    public class ElementEmoji : Element {
        int id;
        /// <summary></summary>
        public ElementEmoji (int id):
            base ("emoji", ("id", id.ToString ())) { this.id = id; }
    }
}