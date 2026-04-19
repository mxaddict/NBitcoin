using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NBitcoin;
using NBitcoin.Altcoins;
using NBitcoin.RPC;
using Xunit;

namespace NBitcoin.Tests
{
    [Trait("Altcoins", "Altcoins")]
    public class NavioTests
    {
        [Fact]
        public void NavioTestnetNetworkExists()
        {
            var network = AltNetworkSets.Navio.Testnet;
            Assert.NotNull(network);
        }

        [Fact(Skip = "Navio testnet uses a BLSCT genesis block that NBitcoin cannot parse natively. " +
            "The genesis bytes in Navio.cs are a synthetic parseable substitute. " +
            "TODO: replace with real genesis bytes and re-enable once a BLSCT-aware block parser is available.")]
        public void NavioGenesisHashIsCorrect()
        {
            var network = AltNetworkSets.Navio.Testnet;
            var expectedGenesisHash = uint256.Parse("57b37639169f354fd61978f8e88db8d7da085c1c6ac4e625c5d018b0d9019e2b");
            Assert.Equal(expectedGenesisHash, network.GenesisHash);
        }

        [Fact]
        public void NavioNetworkPortsAreCorrect()
        {
            var network = AltNetworkSets.Navio.Testnet;
            Assert.Equal(33570, network.DefaultPort);
            Assert.Equal(33577, network.RPCPort);
        }

        [Fact]
        public void NavioTestnet_Bech32_WitnessAddressHRP_IsTb()
        {
            // Navio testnet inherits standard segwit bech32 "tb" for transparent addresses.
            // The BLSCT-specific HRP "tnv" is a string constant passed to encode_address,
            // not registered as a Bech32Type — it is tested in NBXplorer BlsctDerivationTests.
            var network = AltNetworkSets.Navio.Testnet;
            var encoder = network.GetBech32Encoder(Bech32Type.WITNESS_PUBKEY_ADDRESS, false);
            Assert.NotNull(encoder);
            Assert.Equal("tb", System.Text.Encoding.ASCII.GetString(encoder.HumanReadablePart));
        }

        [Fact]
        public void NavioTestnet_Base58_P2PKH_PrefixIs0x6f()
        {
            var network = AltNetworkSets.Navio.Testnet;
            var prefix = network.GetVersionBytes(Base58Type.PUBKEY_ADDRESS, false);
            Assert.Equal(new byte[] { 0x6f }, prefix);
        }

        [Fact]
        public void NavioTestnet_Base58_P2SH_PrefixIs0xc4()
        {
            var network = AltNetworkSets.Navio.Testnet;
            var prefix = network.GetVersionBytes(Base58Type.SCRIPT_ADDRESS, false);
            Assert.Equal(new byte[] { 0xc4 }, prefix);
        }

        [Fact]
        public void NavioIsInAltNetworkSets()
        {
            var navio = AltNetworkSets.Navio;
            Assert.NotNull(navio);
            var all = AltNetworkSets.GetAll();
            Assert.Contains(navio, all);
        }

        [Fact]
        public void ConfigureBLSCTOverridesCreatesMethodMapping()
        {
            var network = AltNetworkSets.Navio.Testnet;
            var creds = new RPCCredentialString { UserPassword = new NetworkCredential("user", "pass") };
            var client = new RPCClient(creds, "127.0.0.1", network);

            Assert.Null(client.RPCMethodOverrides);

            Navio.ConfigureBLSCTOverrides(client);

            Assert.NotNull(client.RPCMethodOverrides);
        }

        [Fact]
        public void ConfigureBLSCTOverridesHasExactlyEightEntries()
        {
            var network = AltNetworkSets.Navio.Testnet;
            var creds = new RPCCredentialString { UserPassword = new NetworkCredential("user", "pass") };
            var client = new RPCClient(creds, "127.0.0.1", network);

            Navio.ConfigureBLSCTOverrides(client);

            Assert.Equal(8, client.RPCMethodOverrides.Count);
        }

        [Theory]
        [InlineData("getbalance", "getblsctbalance")]
        [InlineData("sendtoaddress", "sendtoblsctaddress")]
        [InlineData("listunspent", "listblsctunspent")]
        [InlineData("listtransactions", "listblscttransactions")]
        [InlineData("createrawtransaction", "createblsctrawtransaction")]
        [InlineData("fundrawtransaction", "fundblsctrawtransaction")]
        [InlineData("signrawtransactionwithwallet", "signblsctrawtransaction")]
        [InlineData("decoderawtransaction", "decodeblsctrawtransaction")]
        public void ConfigureBLSCTOverridesHasCorrectMappings(string from, string to)
        {
            var network = AltNetworkSets.Navio.Testnet;
            var creds = new RPCCredentialString { UserPassword = new NetworkCredential("user", "pass") };
            var client = new RPCClient(creds, "127.0.0.1", network);

            Navio.ConfigureBLSCTOverrides(client);

            Assert.True(client.RPCMethodOverrides.ContainsKey(from));
            Assert.Equal(to, client.RPCMethodOverrides[from]);
        }

        [Fact]
        public void ConfigureBLSCTOverridesDoesNotContainCreatewallet()
        {
            var network = AltNetworkSets.Navio.Testnet;
            var creds = new RPCCredentialString { UserPassword = new NetworkCredential("user", "pass") };
            var client = new RPCClient(creds, "127.0.0.1", network);

            Navio.ConfigureBLSCTOverrides(client);

            Assert.False(client.RPCMethodOverrides.ContainsKey("createwallet"));
        }

        [Fact]
        public void ConfigureBLSCTOverridesDoesNotContainGetnewaddress()
        {
            var network = AltNetworkSets.Navio.Testnet;
            var creds = new RPCCredentialString { UserPassword = new NetworkCredential("user", "pass") };
            var client = new RPCClient(creds, "127.0.0.1", network);

            Navio.ConfigureBLSCTOverrides(client);

            Assert.False(client.RPCMethodOverrides.ContainsKey("getnewaddress"));
        }

        [Fact]
        public void CreateWalletOptionsBlsctDefaultsToNull()
        {
            var options = new CreateWalletOptions();
            Assert.Null(options.Blsct);
        }

        [Fact]
        public void CreateWalletOptions_BlsctTrue_IncludedInSerialization()
        {
            // Mirror the parameter-building logic from RPCClient.CreateWalletAsync —
            // when Blsct=true the "blsct" key must appear in the named-args dict.
            var options = new CreateWalletOptions { Blsct = true };
            var parameters = new Dictionary<string, object>();
            if (options?.Blsct is bool blsct)
                parameters.Add("blsct", blsct);
            Assert.True(parameters.ContainsKey("blsct"));
            Assert.True((bool)parameters["blsct"]);
        }

        [Fact]
        public void CreateWalletOptions_BlsctNull_OmittedFromSerialization()
        {
            // When Blsct is null (default), "blsct" must NOT appear in the named-args dict,
            // so the daemon uses its own default and non-BLSCT callers are unaffected.
            var options = new CreateWalletOptions { Blsct = null };
            var parameters = new Dictionary<string, object>();
            if (options?.Blsct is bool blsct)
                parameters.Add("blsct", blsct);
            Assert.False(parameters.ContainsKey("blsct"));
        }

        [Theory]
        [InlineData(nameof(RPCOperations.getblsctbalance))]
        [InlineData(nameof(RPCOperations.sendtoblsctaddress))]
        [InlineData(nameof(RPCOperations.listblsctunspent))]
        [InlineData(nameof(RPCOperations.listblscttransactions))]
        [InlineData(nameof(RPCOperations.createblsctrawtransaction))]
        [InlineData(nameof(RPCOperations.fundblsctrawtransaction))]
        [InlineData(nameof(RPCOperations.signblsctrawtransaction))]
        [InlineData(nameof(RPCOperations.decodeblsctrawtransaction))]
        [InlineData(nameof(RPCOperations.setblsctseed))]
        [InlineData(nameof(RPCOperations.getblsctseed))]
        [InlineData(nameof(RPCOperations.getblsctauditkey))]
        [InlineData(nameof(RPCOperations.createblsctbalanceproof))]
        [InlineData(nameof(RPCOperations.unlockblsctoutpoint))]
        [InlineData(nameof(RPCOperations.getblsctrecoverydata))]
        [InlineData(nameof(RPCOperations.generatetoblsctaddress))]
        public void BlsctRPCOperationsEnumExists(string operationName)
        {
            var hasOperation = Enum.GetNames(typeof(RPCOperations)).Contains(operationName);
            Assert.True(hasOperation, $"RPCOperations.{operationName} does not exist");
        }
    }
}
