# Política de Privacidade - Dev Note

**Última atualização:** 09 de março de 2026

## Introdução

O Dev Note é um aplicativo de notas de voz que transcreve e organiza suas anotações automaticamente. Esta política descreve como tratamos seus dados.

## Dados coletados

### Gravação de áudio
O aplicativo solicita permissão de acesso ao microfone (`RECORD_AUDIO`) exclusivamente para:
- Gravar notas de voz
- Gravar comentários de voz em notas existentes

As gravações de áudio são armazenadas **localmente no seu dispositivo** e enviadas para a API do OpenAI apenas para fins de transcrição (speech-to-text). O áudio **não é armazenado em nossos servidores**.

### Dados de notas
Todas as suas notas, categorias e comentários são armazenados **localmente no dispositivo** em um banco de dados SQLite. Nenhum dado é enviado para servidores próprios.

### Serviços de terceiros
O aplicativo utiliza as seguintes APIs do OpenAI:
- **Whisper** — para transcrição de áudio em texto
- **GPT-4** — para organização e melhoria de notas

Os dados enviados ao OpenAI estão sujeitos à [Política de Privacidade do OpenAI](https://openai.com/privacy). Apenas o conteúdo necessário para o processamento é enviado (áudio para transcrição, texto para interpretação).

## Dados que NÃO coletamos

- Não coletamos dados pessoais de identificação
- Não coletamos dados de localização
- Não utilizamos cookies ou rastreadores
- Não compartilhamos dados com terceiros para fins publicitários
- Não armazenamos dados em servidores próprios

## Armazenamento e segurança

Todos os dados do aplicativo são armazenados localmente no seu dispositivo. A chave da API do OpenAI é configurada pelo próprio usuário e armazenada localmente.

## Exclusão de dados

Você pode excluir todos os seus dados a qualquer momento:
- Excluindo notas individualmente dentro do aplicativo
- Limpando os dados do aplicativo nas configurações do Android
- Desinstalando o aplicativo

## Permissões utilizadas

| Permissão | Finalidade |
|-----------|-----------|
| `RECORD_AUDIO` | Gravação de notas e comentários de voz |
| `INTERNET` | Comunicação com a API do OpenAI para transcrição e interpretação |

## Alterações nesta política

Eventuais alterações serão publicadas nesta página com a data de atualização revisada.

## Contato

Em caso de dúvidas sobre esta política de privacidade, abra uma issue no repositório do projeto: [github.com/landim32/DevNote](https://github.com/landim32/DevNote/issues)
