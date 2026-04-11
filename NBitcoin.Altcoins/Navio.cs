using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using System;
using System.Linq;

namespace NBitcoin.Altcoins
{
    public class Navio : NetworkSetBase
    {
        public static Navio Instance { get; } = new Navio();
        public override string CryptoCode => "NAV";

        private Navio()
        {
        }

        protected override void PostInit()
        {
            RegisterDefaultCookiePath("Navio",
                new FolderName() { TestnetFolder = "testnet5" });
        }

        protected override NetworkBuilder CreateTestnet()
        {
            return new NetworkBuilder()
                .SetConsensus(new Consensus()
                {
                    SubsidyHalvingInterval = 210000,
                    MajorityEnforceBlockUpgrade = 51,
                    MajorityRejectBlockOutdated = 75,
                    MajorityWindow = 100,
                    PowTargetTimespan = TimeSpan.FromSeconds(14 * 24 * 60 * 60),
                    PowTargetSpacing = TimeSpan.FromSeconds(10 * 60),
                    PowAllowMinDifficultyBlocks = true,
                    PowNoRetargeting = true,
                    CoinbaseMaturity = 100,
                    ConsensusFactory = BitcoinConsensusFactory.Instance,
                    SupportSegwit = true,
                    SupportTaproot = true,
                })
                .SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 111 })
                .SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 196 })
                .SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 239 })
                .SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x35, 0x87, 0xCF })
                .SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x35, 0x83, 0x94 })
                .SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("tb"))
                .SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("tb"))
                .SetMagic(0xdf0e3cb9)
                .SetPort(33570)
                .SetRPCPort(33577)
                .SetMaxP2PVersion(70016)
                .SetName("nav-test")
                .SetNetworkStringParser(new BitcoinStringParser())
                .SetGenesis("010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003f354fd6ad7ea5b0e978fae4693d22fda0cd3d8de25d60a1dc50ca40c507f86054bea52edd87ef7366fd103a66f263557736fd006f1a1942dc50ca40c507f20ffff001d0104404e592054696d65732030352f4f63742f32303139205374657665204e61766861695f746573746e657405ffffffff0100f2052a010000004341040184710fa689ad5023690c80f3a49c8f13f8d45b8c857fbcbc8bc4a8e4d3eb4b10f4d4604fa08dce601aaf0f470216fe1b51850b4acf21b179c45070ac7b03a9ac00000000");
        }

        protected override NetworkBuilder CreateMainnet()
        {
            return new NetworkBuilder()
                .SetConsensus(new Consensus()
                {
                    SubsidyHalvingInterval = 210000,
                    MajorityEnforceBlockUpgrade = 750,
                    MajorityRejectBlockOutdated = 950,
                    MajorityWindow = 1000,
                    PowTargetTimespan = TimeSpan.FromSeconds(14 * 24 * 60 * 60),
                    PowTargetSpacing = TimeSpan.FromSeconds(10 * 60),
                    PowAllowMinDifficultyBlocks = false,
                    PowNoRetargeting = false,
                    CoinbaseMaturity = 100,
                    ConsensusFactory = BitcoinConsensusFactory.Instance,
                    SupportSegwit = true,
                    SupportTaproot = true,
                })
                .SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 0 })
                .SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 5 })
                .SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 128 })
                .SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E })
                .SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 })
                .SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("bc"))
                .SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("bc"))
                .SetMagic(0xacb1d2db)
                .SetPort(8333)
                .SetRPCPort(48484)
                .SetMaxP2PVersion(70016)
                .SetName("nav-main")
                .SetNetworkStringParser(new BitcoinStringParser())
                .SetGenesis("00000000e563d370b42d83c98b811fb1bda076dd6c2b01dac9c1e21104c2527786d5c1c6e563d370b42d83c98b811fb1bda076dd6c2b01dac9c1e21104c252770000000000000000000000000000000000000000000000000000000000000000000020ffff001d0104404e592054696d65732030352f4f63742f32303139205374657665204e6176696f5f6d61696e657400ffffffff0100f2052a010000004341040184710fa689ad5023690c80f3a49c8f13f8d45b8c857fbcbc8bc4a8e4d3eb4b10f4d4604fa08dce601aaf0f470216fe1b51850b4acf21b179c45070ac7b03a9ac00000000");
        }

        protected override NetworkBuilder CreateRegtest()
        {
            return new NetworkBuilder()
                .SetConsensus(new Consensus()
                {
                    ConsensusFactory = BitcoinConsensusFactory.Instance,
                    SupportSegwit = true,
                    SupportTaproot = true,
                })
                .SetName("nav-reg")
                .SetGenesis("01000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ffffffffffff001d0104404e592054696d65732030352f4f63746c5f7265677465737405ffffffff0100f2052a010000004341040184710fa689ad5023690c80f3a49c8f13f8d45b8c857fbcbc8bc4a8e4d3eb4b10f4d4604fa08dce601aaf0f470216fe1b51850b4acf21b179c45070ac7b03a9ac00000000");
        }
    }
}