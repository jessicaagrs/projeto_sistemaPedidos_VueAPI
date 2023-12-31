<template>
  <div>
    <v-alert v-if="errorMessage" type="error" dismissible>
      {{ errorMessage }}
    </v-alert>

    <v-toolbar>
      <v-toolbar-title class="text-center">
        Cadastro de Pedidos
      </v-toolbar-title>
    </v-toolbar>

    <v-form v-model="valid">
      <v-container>
        <v-row class="d-flex align-end justify-start">
          <v-col cols="12" md="4">
            <v-textarea
              bg-color="grey-lighten-2"
              v-model="pedidos.descricao"
              label="Detalhes do Pedido"
              rows="1"
              required
            ></v-textarea>
          </v-col>
          <v-col cols="12" md="4">
            <v-select
              :items="produtos"
              density="compact"
              label="Produto"
              item-text="descricao"
              item-value="id"
              v-model="selectedProduto"
              multiple
            ></v-select>
          </v-col>
          <v-col cols="12" md="4">
            <v-text-field
              v-model="pedidos.valor"
              label="Valor:"
              type="number"
            ></v-text-field>
          </v-col>
          <v-col cols="12" md="4">
            <v-btn color="primary" @click="salvarPedido"> Salvar </v-btn>
          </v-col>
        </v-row>
      </v-container>
    </v-form>

    <v-data-table
      v-model="selected"
      :headers="headers"
      :items="pedidos"
      show-select
      return-object
      class="elevation-1"
    >
      <template v-slot:item.valor="{ item }">
        {{ formatCurrency(item.valor) }}
      </template>

      <template v-slot:item.dataCriacao="{ item }">
        {{ formatDate(item.dataCriacao) }}
      </template>
    </v-data-table>

    <v-container class="d-flex justify-start mb-6 bg-surface-variant">
      <v-btn class="ma-2 pa-2" color="error" @click="deletePedido">
        Excluir
      </v-btn>
      <v-btn class="ma-2 pa-2" color="primary" @click="editarPedido">
        Editar
      </v-btn>
    </v-container>
  </div>
</template>

<script>
export default {
  data() {
    return {
      valid: false,
      pedido: {
        descricao: "",
        produtoId: 0,
        valor: 0.0,
        dataCriacao: new Date(),
      },
      headers: [
        {
          text: "Descrição",
          align: "start",
          sortable: false,
          value: "descricao",
        },
        { text: "Valor", align: "start", value: "valor" },
        { text: "Data do Pedido", align: "start", value: "dataCriacao" },
      ],
      pedidos: [],
      produtos: [],
      selected: [],
      idEdicao: 0,
      errorMessage: "",
      selectedProduto: [],
    };
  },
  mounted() {
    this.getPedidos();
    this.getProdutos();
  },
  methods: {
    formatCurrency(value) {
      const numberFormat = new Intl.NumberFormat("pt-BR", {
        style: "currency",
        currency: "BRL",
      });
      return numberFormat.format(value);
    },
    formatDate(date) {
      const options = { year: "numeric", month: "2-digit", day: "2-digit" };
      return new Date(date).toLocaleDateString("pt-BR", options);
    },
    async editarPedido() {
      try {
        if (this.selected.length == 1) {
          const selectedPedido = this.selected[0];
          this.pedidos.descricao = selectedPedido.descricao;
          this.pedidos.valor = selectedPedido.valor;
          this.selectedProduto = selectedPedido.produtoId;
          this.idEdicao = selectedPedido.id;
          this.selected = [];
        } else {
          this.errorMessage =
            "Nenhum item foi selecionado ou mais de uma linha. Nesse caso selecione apenas um.";
        }
      } catch (error) {
        this.errorMessage = error;
      }
    },
    async salvarPedido() {
      try {
        if (
          this.pedidos.descricao != null &&
          this.pedidos.valor != null &&
          this.selectedProduto != null
        ) {
          let contemElemento = this.pedidos.some(
            (item) => item.id === this.idEdicao
          );
          if (contemElemento) {
            this.putPedido();
            this.idEdicao = 0;
          } else {
            this.postPedido();
          }
        } else {
          this.errorMessage =
            "Os campos descrição, valor, data e produto são obrigatórios.";
        }
      } catch (error) {
        this.errorMessage = error;
      }
    },
    async putProduto() {
      try {
        const valorConvertido = parseFloat(
          this.pedidos.valor.replace(",", ".")
        );
        const data = {
          id: this.idEdicao,
          descricao: this.pedidos.descricao,
          produtoId: this.selectedProduto,
          valor: valorConvertido,
        };
        await this.$axios.put("https://localhost:7054/Pedido", data);
        await this.getPedidos();
        this.pedidos.descricao = "";
        this.pedidos.valor = "";
        this.pedidos.dataCriacao = "";
      } catch (error) {
        this.errorMessage = "Erro ao fazer solicitação, revise os dados.";
      }
    },
    async postPedido() {
      try {
        const valorConvertido = parseFloat(
          this.pedidos.valor.replace(",", ".")
        );
        const produtosSelecionados = this.mapearProdutosSelecionados();
        const data = {
          descricao: this.pedidos.descricao,
          valor: valorConvertido,
          produtosPedido: produtosSelecionados,
        };
        await this.$axios.post("https://localhost:7054/Pedido", data);
        await this.getPedidos();
        this.pedidos.descricao = "";
        this.pedidos.valor = "";
        this.pedidos.dataCriacao = "";
      } catch (error) {
        this.errorMessage =
          "Erro ao fazer solicitação, revise os dados.".errors;
      }
    },
    async getProdutos() {
      try {
        const response = await this.$axios.get(
          "https://localhost:7054/Produto"
        );
        this.produtos = response.data;
      } catch (error) {
        this.errorMessage = "Erro ao fazer solicitação, revise os dados.";
      }
    },
    async getPedidos() {
      try {
        const response = await this.$axios.get("https://localhost:7054/Pedido");
        this.pedidos = response.data;
      } catch (error) {
        this.errorMessage = "Erro ao fazer solicitação, revise os dados.";
      }
    },
    async deletePedido() {
      try {
        if (this.selected.length == 1) {
          if (this.selected[0].id) {
            const id = this.selected[0].id;
            await this.$axios.delete(`https://localhost:7054/Pedido/${id}`);
            await this.getPedidos();
          }
        } else {
          this.errorMessage =
            "Nenhum item foi selecionado ou mais de uma linha. Nesse caso selecione apenas um.";
        }
      } catch (error) {
        this.errorMessage = "Erro ao fazer solicitação, revise os dados.";
      }
    },

    mapearProdutosSelecionados() {
      let produtos = this.produtos;
      let produtosSelecionados = [];

      this.selectedProduto.forEach((p) => {
        let existeProduto = produtos.find((x) => x.id === p);
        if (existeProduto) {
          produtosSelecionados.push(existeProduto);
        }
      });

      return produtosSelecionados;
    },
  },
};
</script>
