namespace VMFW.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AcPointInfo",
                c => new
                    {
                        TimeStamp = c.DateTime(nullable: false, precision: 0),
                        WellName = c.String(nullable: false, maxLength: 40, unicode: false),
                        rw = c.Double(nullable: false),
                        ro = c.Double(nullable: false),
                        ud = c.Double(nullable: false),
                        dD = c.Double(nullable: false),
                        ug = c.Double(nullable: false),
                        uw = c.Double(nullable: false),
                        uo = c.Double(nullable: false),
                        rg = c.Double(nullable: false),
                        P = c.Double(nullable: false),
                        To = c.Double(nullable: false),
                        T = c.Double(nullable: false),
                        fw = c.Double(nullable: false),
                        Rr = c.Double(nullable: false),
                        Rs = c.Double(nullable: false),
                        co = c.Double(nullable: false),
                        cg = c.Double(nullable: false),
                        cw = c.Double(nullable: false),
                        cl = c.Double(nullable: false),
                        bdDate = c.DateTime(nullable: false, storeType: "date"),
                        P1 = c.Double(nullable: false),
                        P2 = c.Double(nullable: false),
                        K = c.Double(nullable: false),
                        Re = c.Double(nullable: false),
                        y = c.Double(nullable: false),
                        rl = c.Double(),
                        r = c.Double(),
                        x = c.Double(),
                        ul = c.Double(),
                        u = c.Double(),
                        E = c.Double(),
                        C = c.Double(),
                        a1 = c.Double(),
                        a2 = c.Double(),
                        a3 = c.Double(),
                        a4 = c.Double(),
                        Qol = c.Double(),
                        Qgl = c.Double(),
                        Qwl = c.Double(),
                        Ql = c.Double(),
                        LQO = c.Double(),
                        LQG = c.Double(),
                        LQW = c.Double(),
                        LQL = c.Double(),
                    })
                .PrimaryKey(t => new { t.TimeStamp, t.WellName });
            
            CreateTable(
                "dbo.PointTB",
                c => new
                    {
                        stationName = c.String(nullable: false, maxLength: 40, unicode: false),
                        rw = c.String(nullable: false, maxLength: 40, unicode: false),
                        ro = c.String(nullable: false, maxLength: 40, unicode: false),
                        ud = c.String(nullable: false, maxLength: 40, unicode: false),
                        dD = c.String(nullable: false, maxLength: 40, unicode: false),
                        ug = c.String(nullable: false, maxLength: 40, unicode: false),
                        uw = c.String(nullable: false, maxLength: 40, unicode: false),
                        uo = c.String(nullable: false, maxLength: 40, unicode: false),
                        rg = c.String(nullable: false, maxLength: 40, unicode: false),
                        P = c.String(nullable: false, maxLength: 40, unicode: false),
                        To = c.String(nullable: false, maxLength: 40, unicode: false),
                        T = c.String(nullable: false, maxLength: 40, unicode: false),
                        fw = c.String(nullable: false, maxLength: 40, unicode: false),
                        Rr = c.String(nullable: false, maxLength: 40, unicode: false),
                        Rs = c.String(nullable: false, maxLength: 40, unicode: false),
                        co = c.String(nullable: false, maxLength: 40, unicode: false),
                        cw = c.String(nullable: false, maxLength: 40, unicode: false),
                        cg = c.String(nullable: false, maxLength: 40, unicode: false),
                        cl = c.String(nullable: false, maxLength: 40, unicode: false),
                        year = c.String(nullable: false, maxLength: 40, unicode: false),
                        month = c.String(nullable: false, maxLength: 40, unicode: false),
                        day = c.String(nullable: false, maxLength: 40, unicode: false),
                        P1 = c.String(nullable: false, maxLength: 40, unicode: false),
                        P2 = c.String(nullable: false, maxLength: 40, unicode: false),
                        K = c.String(nullable: false, maxLength: 40, unicode: false),
                        Re = c.String(nullable: false, maxLength: 40, unicode: false),
                        y = c.String(nullable: false, maxLength: 40, unicode: false),
                        unitType = c.String(nullable: false, maxLength: 40, unicode: false),
                        rl = c.String(nullable: false, maxLength: 40, unicode: false),
                        r = c.String(nullable: false, maxLength: 40, unicode: false),
                        x = c.String(nullable: false, maxLength: 40, unicode: false),
                        ul = c.String(nullable: false, maxLength: 40, unicode: false),
                        u = c.String(nullable: false, maxLength: 40, unicode: false),
                        E = c.String(nullable: false, maxLength: 40, unicode: false),
                        C = c.String(nullable: false, maxLength: 40, unicode: false),
                        a1 = c.String(nullable: false, maxLength: 40, unicode: false),
                        a2 = c.String(nullable: false, maxLength: 40, unicode: false),
                        a3 = c.String(nullable: false, maxLength: 40, unicode: false),
                        a4 = c.String(nullable: false, maxLength: 40, unicode: false),
                        Qol = c.String(nullable: false, maxLength: 40, unicode: false),
                        Qwl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Qgl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Ql = c.String(nullable: false, maxLength: 40, unicode: false),
                        Dol = c.String(nullable: false, maxLength: 40, unicode: false),
                        Dwl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Dgl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Dl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Mol = c.String(nullable: false, maxLength: 40, unicode: false),
                        Mwl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Mgl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Ml = c.String(nullable: false, maxLength: 40, unicode: false),
                        Yol = c.String(nullable: false, maxLength: 40, unicode: false),
                        Ywl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Ygl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Yl = c.String(nullable: false, maxLength: 40, unicode: false),
                        EHol = c.String(nullable: false, maxLength: 40, unicode: false),
                        EHwl = c.String(nullable: false, maxLength: 40, unicode: false),
                        EHgl = c.String(nullable: false, maxLength: 40, unicode: false),
                        EHl = c.String(nullable: false, maxLength: 40, unicode: false),
                        EDol = c.String(nullable: false, maxLength: 40, unicode: false),
                        EDwl = c.String(nullable: false, maxLength: 40, unicode: false),
                        EDgl = c.String(nullable: false, maxLength: 40, unicode: false),
                        EDl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Tol = c.String(nullable: false, maxLength: 40, unicode: false),
                        Twl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Tgl = c.String(nullable: false, maxLength: 40, unicode: false),
                        Tl = c.String(nullable: false, maxLength: 40, unicode: false),
                    })
                .PrimaryKey(t => t.stationName);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PointTB");
            DropTable("dbo.AcPointInfo");
        }
    }
}
