<template>
  <div>
    <el-row>
      <el-col :span="5">
        <div class="grid-content bg-purple">
          <TreeView v-on:node-click="OnNodeClick"></TreeView>
        </div>
      </el-col>
      <el-col :span="19">
        <div class="grid-content bg-purple-light">
          <RightTable :treeNodeId="treeNodeId" ref="rightTable"></RightTable>
        </div>
      </el-col>
    </el-row>

  </div>
</template>

<script>
  import {
    RightTable,
    TreeView
  } from "./components";

  export default {
    name: "autoFrom",
    components: {
      RightTable,
      TreeView
    },
    data() {
      return {
        treeNodeId: ""
      };
    },
    methods: {
      OnNodeClick: function (id) {
        this.treeNodeId = id;
        this.$nextTick(() => {
          this.$refs.rightTable.$refs.query.fetchData();
        });
      }
    }
  };

</script>
