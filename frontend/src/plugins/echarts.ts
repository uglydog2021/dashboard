// vue-echarts
import ECharts from 'vue-echarts'
import { use } from "echarts/core"
import {
    CanvasRenderer
  } from 'echarts/renderers'
  import {
    BarChart,PieChart,MapChart,EffectScatterChart,LineChart,GaugeChart
  } from 'echarts/charts'
  import {
    GridComponent,
    TitleComponent,
    TooltipComponent,
    LegendComponent,
    DatasetComponent,
    VisualMapComponent,
    GeoComponent,
    ToolboxComponent,
    MarkPointComponent
  } from 'echarts/components'

  use([
    CanvasRenderer,
    BarChart,PieChart,MapChart,EffectScatterChart,LineChart,GaugeChart,
    GridComponent,
    LegendComponent,
    TooltipComponent,
    TitleComponent,
    DatasetComponent,
    VisualMapComponent,
    GeoComponent,
    ToolboxComponent,
    MarkPointComponent
  ])

  export const registerEcharts= (app:any)=>{
    app.component('v-chart', ECharts)
  }