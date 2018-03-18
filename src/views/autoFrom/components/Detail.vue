<template>
  <div class="container">
    <el-dialog :title="title" :visible.sync="visiable">
      <el-form ref="dataForm" :model="temp" label-position="left" label-width="100px" style='width: 550px; margin-left:50px;'>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="字段名:" style="font-size:15px">
              {{temp.columnName}}
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="数据类型:">
              {{temp.columnType}}
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="字节长度:">
              {{temp.byteLength}}
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="字符长度:">
              {{temp.charLength}}
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="精度:">
              {{temp.scale}}
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="默认值:">
              {{temp.defaultValue}}
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="允许空:">
              {{temp.isNullable ? "是" : "否"}}
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="主键:">
              {{temp.isPrimaryKey ? "是" : "否"}}
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="自增:">
              {{temp.isIdentity ? "是" : "否"}}
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="注释:">
              {{temp.remark}}
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="cancel">{{$t('table.cancel')}}</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import {
  fetchColumnList,
  createArticle,
  updateArticle,
  fetchModelById
} from "@/api/article";

export default {
  name: "Detail",
  props: {
    formId: {
      type: String,
      default: null
    },
    detailVisiable: {
      type: Boolean,
      default: false
    }
  },
  computed: {
    visiable: {
      get: function() {
        if (this.detailVisiable) {
          fetchModelById({
            Id: this.formId
          }).then(response => {
            this.temp = response.data.result;
          });
        }
        return this.detailVisiable;
      },
      set: function(val) {
        this.cancel();
      }
    }
  },
  data() {
    return {
      title: "XXX详情",
      temp: {
        id: "",
        dbId: "",
        columnName: "",
        columnCNName: "",
        columnType: "",
        byteLength: -1,
        charLength: -1,
        scale: -1,
        defaultValue: "",
        remark: "",
        isNullable: false,
        isPrimaryKey: false,
        isIdentity: false
      }
    };
  },
  methods: {
    cancel: function() {
      this.$emit("detail-visiable-change", false);
    }
  }
};
</script>