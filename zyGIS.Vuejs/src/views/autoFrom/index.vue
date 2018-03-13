<template>
  <div>
    <el-row>
      <el-col :span="5"><div class="grid-content bg-purple"><TreeView></TreeView></div></el-col>
      <el-col :span="19"><div class="grid-content bg-purple-light"><RightTable></RightTable></div></el-col>
    </el-row>
    
    
  </div>
</template>

<script>
import { RightTable,TreeView } from './components'

export default {
  name: 'autoFrom',
  components: {
    RightTable,
    TreeView
  },
  data() {
    return {
      
    }
  }
}
</script>