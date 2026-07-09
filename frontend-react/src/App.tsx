import { useState, useEffect } from 'react';
import axios from 'axios';

const API_URL_PESSOA = 'http://localhost:5262/api/pessoa';
const API_URL_TRANSACAO = 'http://localhost:5262/api/transacao';
const API_URL_RELATORIO = 'http://localhost:5262/api/relatorio';

export default function App() {
  // PESSOAS
  const [pessoas, setPessoas] = useState([]);
  const [nome, setNome] = useState('');
  const [idade, setIdade] = useState('');
  const [idEdicao, setIdEdicao] = useState(null);
  const [erro, setErro] = useState('');

  // TRANSACOES
  const [transacoes, setTransacoes] = useState([]);
  const [idEdicaoTransacao, setIdEdicaoTransacao] = useState(null);
  const [descricao, setDescricao] = useState('');
  const [valor, setValor] = useState(0);
  const [tipo, setTipo] = useState('CREDITO');
  const [idPessoaTransacao, setIdPessoaTransacao] = useState('');

  // RELATORIOS (Simplificados para objetos)
  const [relatorioGeral, setRelatorioGeral] = useState(null);
  const [relatorioPessoal, setRelatorioPessoal] = useState(null);
  const [idFiltroPessoal, setIdFiltroPessoal] = useState('');
  const [relatoriosPessoais, setRelatoriosPessoais] = useState([]);

  // Carrega os dados iniciais
  useEffect(() => {
    carregarPessoas();
    carregarTransacoes();
    carregarRelatorioGeral();
    carregarTodosRelatoriosPessoais();
  }, []);

  const carregarPessoas = async () => {
    try {
      const res = await axios.get(API_URL_PESSOA);
      setPessoas(res.data);
    } catch (err) {
      setErro('Erro ao carregar a lista de pessoas.');
    }
  };

  const carregarTransacoes = async () => {
    try {
      const res = await axios.get(API_URL_TRANSACAO);
      setTransacoes(res.data);
    } catch (err) {
      setErro("Erro ao carregar a lista de transações.");
    }
  };

  const carregarRelatorioGeral = async () => {
    try {
      const res = await axios.get(`${API_URL_RELATORIO}/geral`);
      setRelatorioGeral(res.data);
    } catch (err) {
      setErro("Erro ao carregar relatório geral.");
    }
  };

  // Carrega a lista completa vinda da sua API
  const carregarTodosRelatoriosPessoais = async () => {
    try {
      const res = await axios.get(`${API_URL_RELATORIO}/pessoal`);
      setRelatoriosPessoais(res.data); // Guarda o array completo no estado
    } catch (err) {
      setErro("Erro ao carregar relatórios pessoais.");
    }
  };

  // Em vez de ir na API, essa função apenas filtra o array baseado no ID selecionado
  const buscarRelatorioPessoal = (id) => {
    if (!id) {
      setRelatorioPessoal(null);
      return;
    }

    const encontrado = relatoriosPessoais.find(r => {
      const rId = r.id ?? r.Id ?? r.ID;
      return Number(rId) === Number(id);
    });

    setRelatorioPessoal(encontrado || null);
  };

  const salvarPessoa = async (e) => {
    e.preventDefault();
    setErro('');
    const payload = { nome, idade: Number(idade) };
    try {
      if (idEdicao) {
        await axios.put(`${API_URL_PESSOA}/${idEdicao}`, payload);
        setIdEdicao(null);
      } else {
        await axios.post(API_URL_PESSOA, payload);
      }
      setNome('');
      setIdade('');
      carregarPessoas();
    } catch (err) {
      setErro(err.response?.data || 'Erro ao salvar os dados do usuário.');
    }
  };

  const salvarTransacao = async (e) => {
    e.preventDefault();
    setErro('');
    const payload = {
      descricao,
      valor: Number(valor),
      tipo,
      id_Pessoa: Number(idPessoaTransacao)
    };
    try {
      if (idEdicaoTransacao) {
        await axios.put(`${API_URL_TRANSACAO}/${idEdicaoTransacao}`, payload);
        setIdEdicaoTransacao(null);
      } else {
        await axios.post(API_URL_TRANSACAO, payload);
      }
      setDescricao('');
      setValor(0);
      setTipo('CREDITO');
      setIdPessoaTransacao('');

      // Atualiza tudo
      await carregarTransacoes();
      await carregarRelatorioGeral();

      // CORREÇÃO: Busca a lista nova antes de filtrar
      try {
        const res = await axios.get(`${API_URL_RELATORIO}/pessoal`);
        setRelatoriosPessoais(res.data);
        if (idFiltroPessoal) {
          const encontrado = res.data.find(r => (r.id ?? r.Id ?? r.ID) === Number(idFiltroPessoal));
          setRelatorioPessoal(encontrado || null);
        }
      } catch {
        setErro("Erro ao atualizar relatórios.");
      }

    } catch (err) {
      setErro(err.response?.data || 'Erro ao salvar os dados da transação.');
    }
  };

  const iniciarEdicaoPessoa = (pessoa) => {
    setIdEdicao(pessoa.id);
    setNome(pessoa.nome);
    setIdade(pessoa.idade);
  };

  const iniciarEdicaoTransacao = (transacao) => {
    setIdEdicaoTransacao(transacao.id);
    setDescricao(transacao.descricao);
    setValor(transacao.valor);
    setTipo(transacao.tipo);
    setIdPessoaTransacao(transacao.id_Pessoa || transacao.idPessoaTransacao);
  };

  const cancelarEdicaoPessoa = () => {
    setIdEdicao(null);
    setNome('');
    setIdade('');
  };

  const cancelarEdicaoTransacao = () => {
    setIdEdicaoTransacao(null);
    setDescricao('');
    setValor(0);
    setTipo('CREDITO');
    setIdPessoaTransacao('');
  };

  const deletarPessoa = async (id) => {
    if (!confirm('Deseja realmente excluir esta pessoa?')) return;
    try {
      await axios.delete(`${API_URL_PESSOA}/${id}`);
      carregarPessoas();
      carregarRelatorioGeral();
    } catch (err) {
      setErro(err.response?.data || 'Erro ao deletar pessoa.');
    }
  };

  const deletarTransacao = async (id) => {
    if (!confirm('Deseja realmente excluir esta transação?')) return;
    try {
      await axios.delete(`${API_URL_TRANSACAO}/${id}`);
      await carregarTransacoes();
      await carregarRelatorioGeral();

      // CORREÇÃO: Atualiza os relatórios pós-deleção
      const res = await axios.get(`${API_URL_RELATORIO}/pessoal`);
      setRelatoriosPessoais(res.data);
      if (idFiltroPessoal) {
        const encontrado = res.data.find(r => (r.id ?? r.Id ?? r.ID) === Number(idFiltroPessoal));
        setRelatorioPessoal(encontrado || null);
      }
    } catch (err) {
      setErro(err.response?.data || 'Erro ao deletar Transação.');
    }
  };

  return (
    <div style={{ backgroundColor: '#f4f6f9', minHeight: '100vh', padding: '30px 0', fontFamily: '"Segoe UI", Roboto, sans-serif' }}>
      {/* CONTAINER EXPANDIDO - Ocupa toda a largura da tela */}
      <div style={{ width: '100%', maxWidth: '100%', padding: '0 30px', boxSizing: 'border-box' }}>

        <h1 style={{ color: '#1e293b', marginBottom: '30px', fontSize: '28px', fontWeight: '700' }}>
          📊 Desafio Técnico: Sistema de Fluxo de Caixa
        </h1>

        {erro && (
          <p style={{ color: '#991b1b', backgroundColor: '#fee2e2', borderLeft: '4px solid #ef4444', padding: '12px 16px', borderRadius: '6px', marginBottom: '25px', fontWeight: '550' }}>
            {erro}
          </p>
        )}

        {/* DASHBOARD DE RELATÓRIOS */}
        <div style={{ display: 'flex', gap: '24px', marginBottom: '40px', flexWrap: 'wrap' }}>
          {/* Card Geral */}
          <div style={{ flex: '1 1 500px', minWidth: '280px', backgroundColor: '#ffffff', padding: '24px', borderRadius: '12px', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.05), 0 2px 4px -1px rgba(0,0,0,0.03)' }}>
            <h3 style={{ margin: '0 0 16px 0', color: '#475569', fontSize: '16px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>Relatório Geral</h3>
            {relatorioGeral ? (
              <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                  <span style={{ color: '#64748b' }}>Receitas:</span>
                  <span style={{ color: '#10b981', fontWeight: '600' }}>R$ {relatorioGeral.totalReceitas}</span>
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                  <span style={{ color: '#64748b' }}>Despesas:</span>
                  <span style={{ color: '#ef4444', fontWeight: '600' }}>R$ {relatorioGeral.totalDespesas}</span>
                </div>
                <div style={{ borderTop: '1px solid #e2e8f0', paddingTop: '12px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <span style={{ color: '#1e293b', fontWeight: '500' }}>Saldo Geral:</span>
                  <span style={{ fontSize: '18px', color: '#1e293b', fontWeight: '700' }}>R$ {relatorioGeral.saldoGeral}</span>
                </div>
              </div>
            ) : <p style={{ color: '#64748b' }}>Carregando...</p>}
          </div>

          {/* Card Pessoal */}
          <div style={{ flex: '1 1 500px', minWidth: '280px', backgroundColor: '#ffffff', padding: '24px', borderRadius: '12px', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.05), 0 2px 4px -1px rgba(0,0,0,0.03)' }}>
            <h3 style={{ margin: '0 0 16px 0', color: '#475569', fontSize: '16px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>Relatório Pessoal</h3>
            <select
              value={idFiltroPessoal}
              onChange={e => { setIdFiltroPessoal(e.target.value); buscarRelatorioPessoal(e.target.value); }}
              style={{ width: '100%', padding: '10px', borderRadius: '6px', border: '1px solid #cbd5e1', backgroundColor: '#f8fafc', color: '#334155', cursor: 'pointer', outline: 'none' }}
            >
              <option value="">Selecione uma pessoa</option>
              {pessoas.map(p => <option key={p.id} value={p.id}>{p.nome}</option>)}
            </select>

            {relatorioPessoal ? (
              <div style={{ display: 'flex', flexDirection: 'column', gap: '12px', marginTop: '16px' }}>
                <p style={{ margin: 0, color: '#1e293b' }}>Nome: <strong>{relatorioPessoal.Nome || relatorioPessoal.nome}</strong></p>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                  <span style={{ color: '#64748b' }}>Receitas:</span>
                  <span style={{ color: '#10b981', fontWeight: '600' }}>R$ {relatorioPessoal.TotalReceitas ?? relatorioPessoal.totalReceitas}</span>
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                  <span style={{ color: '#64748b' }}>Despesas:</span>
                  <span style={{ color: '#ef4444', fontWeight: '600' }}>R$ {relatorioPessoal.TotalDespesas ?? relatorioPessoal.totalDespesas}</span>
                </div>
                <div style={{ borderTop: '1px solid #e2e8f0', paddingTop: '12px', display: 'flex', justifyContent: 'space-between' }}>
                  <span style={{ color: '#1e293b', fontWeight: '500' }}>Saldo Pessoal:</span>
                  <span style={{ color: '#1e293b', fontWeight: '700' }}>R$ {relatorioPessoal.Saldo ?? relatorioPessoal.saldo}</span>
                </div>
              </div>
            ) : <p style={{ marginTop: '16px', color: '#94a3b8', fontSize: '14px' }}>Selecione alguém para visualizar os dados.</p>}
          </div>
        </div>

        {/* CORPO DA TELA - DUAS COLUNAS AMPLAS */}
        <div style={{ display: 'flex', gap: '32px', flexWrap: 'wrap' }}>

          {/* COLUNA 1: PESSOAS */}
          <div style={{ flex: '1 1 500px', backgroundColor: '#ffffff', padding: '24px', borderRadius: '12px', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.05)' }}>
            <h2 style={{ color: '#1e293b', fontSize: '20px', marginBottom: '20px', fontWeight: '600' }}>Gerenciar Pessoas</h2>

            <form onSubmit={salvarPessoa} style={{ display: 'flex', gap: '12px', marginBottom: '24px' }}>
              <input type="text" placeholder="Nome" value={nome} onChange={e => setNome(e.target.value)} required style={{ flex: 2, padding: '10px 14px', borderRadius: '6px', border: '1px solid #cbd5e1', outline: 'none', backgroundColor: '#1e293b' }} />
              <input type="number" placeholder="Idade" value={idade} onChange={e => setIdade(e.target.value)} required style={{ flex: 1, padding: '10px 14px', borderRadius: '6px', border: '1px solid #cbd5e1', outline: 'none', backgroundColor: '#1e293b' }} />
              <button type="submit" style={{ backgroundColor: '#2563eb', color: '#ffffff', border: 'none', padding: '10px 18px', borderRadius: '6px', fontWeight: '600', cursor: 'pointer' }}>
                {idEdicao ? 'Atualizar' : 'Cadastrar'}
              </button>
              {idEdicao && <button type="button" onClick={cancelarEdicaoPessoa} style={{ backgroundColor: '#64748b', color: '#ffffff', border: 'none', padding: '10px 14px', borderRadius: '6px', cursor: 'pointer' }}>X</button>}
            </form>

            <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
              <thead>
                <tr style={{ borderBottom: '2px solid #edf2f7', color: '#475569', fontSize: '14px' }}>
                  <th style={{ padding: '12px 8px' }}>ID</th>
                  <th style={{ padding: '12px 8px' }}>Nome</th>
                  <th style={{ padding: '12px 8px' }}>Idade</th>
                  <th style={{ padding: '12px 8px', textAlign: 'right' }}>Ações</th>
                </tr>
              </thead>
              <tbody style={{ color: '#334155', fontSize: '15px' }}>
                {pessoas.map(p => (
                  <tr key={p.id} style={{ borderBottom: '1px solid #f1f5f9' }}>
                    <td style={{ padding: '14px 8px' }}>{p.id}</td>
                    <td style={{ padding: '14px 8px', fontWeight: '500' }}>{p.nome}</td>
                    <td style={{ padding: '14px 8px' }}>{p.idade} anos</td>
                    <td style={{ padding: '14px 8px', textAlign: 'right' }}>
                      <button onClick={() => iniciarEdicaoPessoa(p)} style={{ backgroundColor: 'transparent', color: '#2563eb', border: 'none', marginRight: '12px', fontWeight: '600', cursor: 'pointer' }}>Editar</button>
                      <button onClick={() => deletarPessoa(p.id)} style={{ backgroundColor: 'transparent', color: '#ef4444', border: 'none', fontWeight: '600', cursor: 'pointer' }}>Excluir</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* COLUNA 2: TRANSAÇÕES */}
          <div style={{ flex: '1 1 500px', backgroundColor: '#ffffff', padding: '24px', borderRadius: '12px', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.05)' }}>
            <h2 style={{ color: '#1e293b', fontSize: '20px', marginBottom: '20px', fontWeight: '600' }}>Gerenciar Transações</h2>

            <form onSubmit={salvarTransacao} style={{ display: 'flex', flexDirection: 'column', gap: '12px', marginBottom: '24px' }}>
              <div style={{ display: 'flex', gap: '12px' }}>
                <input type="text" placeholder="Descrição" value={descricao} onChange={e => setDescricao(e.target.value)} required style={{ flex: 2, padding: '10px 14px', borderRadius: '6px', border: '1px solid #cbd5e1', outline: 'none', backgroundColor: '#1e293b' }} />
                <input type="number" placeholder="Valor" value={valor} onChange={e => setValor(e.target.value)} required style={{ flex: 1, padding: '10px 14px', borderRadius: '6px', border: '1px solid #cbd5e1', outline: 'none', backgroundColor: '#1e293b' }} />
              </div>
              <div style={{ display: 'flex', gap: '12px' }}>
                <select value={tipo} onChange={e => setTipo(e.target.value)} style={{ flex: 1, padding: '10px', borderRadius: '6px', border: '1px solid #cbd5e1', backgroundColor: '#1e293b', cursor: 'pointer' }}>
                  <option value="CREDITO">CREDITO (Receita)</option>
                  <option value="DEBITO">DEBITO (Despesa)</option>
                </select>
                <select value={idPessoaTransacao} onChange={e => setIdPessoaTransacao(e.target.value)} required style={{ flex: 1, padding: '10px', borderRadius: '6px', border: '1px solid #cbd5e1', backgroundColor: '#1e293b', cursor: 'pointer' }}>
                  <option value="">Vincular a uma pessoa</option>
                  {pessoas.map(p => <option key={p.id} value={p.id}>{p.nome}</option>)}
                </select>
                <button type="submit" style={{ backgroundColor: '#059669', color: '#ffffff', border: 'none', padding: '10px 20px', borderRadius: '6px', fontWeight: '600', cursor: 'pointer' }}>
                  {idEdicaoTransacao ? 'Atualizar' : 'Lançar'}
                </button>
                {idEdicaoTransacao && <button type="button" onClick={cancelarEdicaoTransacao} style={{ backgroundColor: '#64748b', color: '#ffffff', border: 'none', padding: '10px 14px', borderRadius: '6px', cursor: 'pointer' }}>X</button>}
              </div>
            </form>

            <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
              <thead>
                <tr style={{ borderBottom: '2px solid #edf2f7', color: '#475569', fontSize: '14px' }}>
                  <th style={{ padding: '12px 8px' }}>Descrição</th>
                  <th style={{ padding: '12px 8px' }}>Valor</th>
                  <th style={{ padding: '12px 8px' }}>Tipo</th>
                  <th style={{ padding: '12px 8px' }}>Pessoa</th>
                  <th style={{ padding: '12px 8px', textAlign: 'right' }}>Ações</th>
                </tr>
              </thead>
              <tbody style={{ color: '#334155', fontSize: '15px' }}>
                {transacoes.map(t => {
                  const vinculo = pessoas.find(p => p.id === (t.id_Pessoa || t.idPessoaTransacao));
                  return (
                    <tr key={t.id} style={{ borderBottom: '1px solid #f1f5f9' }}>
                      <td style={{ padding: '14px 8px', fontWeight: '500' }}>{t.descricao}</td>
                      <td style={{ padding: '14px 8px' }}>R$ {t.valor}</td>
                      <td style={{ padding: '14px 8px' }}>
                        <span style={{ backgroundColor: t.tipo === 'CREDITO' ? '#d1fae5' : '#fee2e2', color: t.tipo === 'CREDITO' ? '#065f46' : '#991b1b', padding: '4px 8px', borderRadius: '4px', fontSize: '12px', fontWeight: '700' }}>
                          {t.tipo}
                        </span>
                      </td>
                      <td style={{ padding: '14px 8px', color: '#64748b', fontSize: '14px' }}>
                        {vinculo ? `${vinculo.nome} (ID: ${vinculo.id})` : 'Não vinculada'}
                      </td>
                      <td style={{ padding: '14px 8px', textAlign: 'right' }}>
                        <button onClick={() => iniciarEdicaoTransacao(t)} style={{ backgroundColor: 'transparent', color: '#2563eb', border: 'none', marginRight: '12px', fontWeight: '600', cursor: 'pointer' }}>Editar</button>
                        <button onClick={() => deletarTransacao(t.id)} style={{ backgroundColor: 'transparent', color: '#ef4444', border: 'none', fontWeight: '600', cursor: 'pointer' }}>Excluir</button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

        </div>
      </div>
    </div>
  );
}
