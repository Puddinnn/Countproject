<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PointCurve.aspx.cs" Inherits="Charts_PointCurve" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../Scripts/jquery.min.js"></script>
    <script src="../Scripts/echarts.min.js"></script>
    <style type="text/css">
        #btnQuery {
            color: #FFFFFF;
            background-color: #3399FF;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
            <asp:Menu ID="Menu1" runat="server" BackColor="#E3EAEB" DynamicHorizontalOffset="2" Font-Bold="True" Font-Names="微軟正黑體" Font-Size="Large" ForeColor="#666666" Orientation="Horizontal" StaticSubMenuIndent="20px" Width="270px">
            <DynamicHoverStyle BackColor="#666666" ForeColor="White" />
            <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <DynamicMenuStyle BackColor="#E3EAEB" />
            <DynamicSelectedStyle BackColor="#1C5E55" />
            <Items>
                <asp:MenuItem NavigateUrl="~/Home_page.aspx" Text="主页" Value="主页"></asp:MenuItem>
                <asp:MenuItem Text="信息" Value="信息">
                    <asp:MenuItem Text="个人信息" Value="个人信息"></asp:MenuItem>
                </asp:MenuItem>
                <asp:MenuItem Text="报表" Value="报表">
                    <asp:MenuItem Text="测点信息" Value="测点信息" NavigateUrl="~/Point_info.aspx"></asp:MenuItem>
                    <asp:MenuItem Text="历史报表" Value="历史报表">
                        <asp:MenuItem Text="历史报表" Value="历史报表" NavigateUrl="~/Historical_Statement.aspx"></asp:MenuItem>
                        <asp:MenuItem Text="历史统计数据(V2.0)" Value="历史统计数据(V2.0)" NavigateUrl="~/Historical_StatmentV2.aspx"></asp:MenuItem>
                    </asp:MenuItem>
                    <asp:MenuItem Text="实时报表" Value="实时报表"></asp:MenuItem>
                </asp:MenuItem>
                <asp:MenuItem Text="曲线" Value="曲线">
                    <asp:MenuItem Text="历史曲线" Value="历史曲线">
                        <asp:MenuItem Text="历史曲线" Value="历史曲线" NavigateUrl="~/Charts/PointCurve.aspx"></asp:MenuItem>
                        <asp:MenuItem Text="历史统计值曲线(V2.0)" Value="历史统计值曲线(V2.0)"></asp:MenuItem>
                    </asp:MenuItem>
                    <asp:MenuItem Text="实时曲线" Value="实时曲线"></asp:MenuItem>
                </asp:MenuItem>
            </Items>
            <StaticHoverStyle BackColor="#666666" ForeColor="White" />
            <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <StaticSelectedStyle BackColor="#1C5E55" />
        </asp:Menu>
                <h1>
                <asp:Label ID="Label1" runat="server" Text="历史曲线"></asp:Label>
            </h1>
        <br />
        <br />
            选择测点：<asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="SqlDataSource1" DataTextField="M_ID" DataValueField="M_ID">
            </asp:DropDownList>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:KJ90X_TestConnectionString %>" SelectCommand="SELECT [M_ID] FROM [Device_Info_Test]"></asp:SqlDataSource>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 开始时间：<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 结束时间：<asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <input type="button" id="btnQuery" onclick ="drawChart()" value="点击查询曲线"/><br />
&nbsp;<div id="main1" style="width: 1000px; height: 400px; border: 1px solid #ccc"></div>
    <%--<div id ="data" style="width:1000px;height:200px; border: 1px solid #ccc"></div>--%>
            <br />
            <br />
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="返回" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="退出登录" />
&nbsp;&nbsp;
    </form>
    <script type="text/javascript">
        $(function () {
            //drawChart();
        });

        function drawChart() {

            //jQuery.parseJSON(jsonString) 
            //$.post("GetPointCurveData.aspx?M_id=" + $("#pointIdText").val(), function (data) {
            //$.post("GetPointCurveData.aspx?M_id=" + $("#DropDownList1").val(), function (data) {
            $.post("GetPointCurveData.aspx?M_id=" + $("#DropDownList1").val() + "&Start_time=" + $("#TextBox1").val() + "&Final_time=" + $("#TextBox2").val(), function (data) {
                var myChart = echarts.init(document.getElementById('main1'));
                //alert(data);
                $("#data").html(data);
                if (data == null || data.length < 1) {

                    // 基于准备好的dom，初始化echarts实例
                    myChart.clear();

                    alert("查询未得到数据！");
                    return;
                }
                var jsonObject = $.parseJSON(data);
                var yValues = new Array();
                var xValues = new Array();

                for (var i = 0; i < jsonObject.data.length; i++) {
                    xValues.push(jsonObject.data[i].D_Generated_Time);
                    yValues.push(jsonObject.data[i].D_Value);
                }

                // 指定图表的配置项和数据
                var option = {
                    title: {
                        text: '曲线'
                    },
                    tooltip: {
                    trigger: 'axis'
                    },
                    legend: {
                        data: ['监测值']
                    },
                    xAxis: {
                        data: xValues,
                        boundaryGap: true
                    },
                    yAxis: {},
                    series: [{
                        name: '监测值',
                        type: 'line',
                        data: yValues
                    }]
                };

                // 使用刚指定的配置项和数据显示图表。
                myChart.setOption(option);

            });

            //// 基于准备好的dom，初始化echarts实例
            //var myChart = echarts.init(document.getElementById('main1'));

            //// 指定图表的配置项和数据
            //var option = {
            //    title: {
            //        text: 'ECharts 入门示例'
            //    },
            //    tooltip: {},
            //    legend: {
            //        data: ['销量']
            //    },
            //    xAxis: {
            //        data: ["衬衫", "羊毛衫", "雪纺衫", "裤子", "高跟鞋", "袜子"]
            //    },
            //    yAxis: {},
            //    series: [{
            //        name: '销量',
            //        type: 'line',
            //        data: [5, 20, 36, 10, 10, 20]
            //    }]
            //};

            //// 使用刚指定的配置项和数据显示图表。
            //myChart.setOption(option);
        }
    </script>
</body>
</html>
