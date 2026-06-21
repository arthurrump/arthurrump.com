{
  inputs = {
    flake-parts.url = "github:hercules-ci/flake-parts";
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";

    picocss = {
      url = "github:picocss/pico/v2.1.1";
      flake = false;
    };
  };

  outputs = inputs@{ self, flake-parts, ... }:
    flake-parts.lib.mkFlake { inherit inputs; } {
      systems = [ "x86_64-linux" "aarch64-linux" "aarch64-darwin" "x86_64-darwin" ];
      perSystem = { config, self', inputs', pkgs, system, ... }: 
      let 
        tools = with pkgs; [
          hugo
          dart-sass
        ];

        sassCmd = "sass theme/style/style.scss static/theme/css/style.css --no-source-map";

        SASS_PATH = "${inputs.picocss}/scss/";
      in {
        devShells.default = pkgs.mkShell {
          packages = tools ++ [
            (pkgs.writeShellScriptBin "develop" ''
              ${sassCmd} --watch &
              hugo server --buildDrafts --navigateToChanged
            '')
          ];
          inherit SASS_PATH;
        };

        packages.default = pkgs.stdenv.mkDerivation {
          name = "site";
          src = ./.;
          buildInputs = tools;
          inherit SASS_PATH;
          phases = [ "unpackPhase" "buildPhase" "installPhase" ];
          buildPhase = ''
            ${sassCmd}
            hugo --minify
          '';
          installPhase = ''
            mkdir -p $out
            cp -r ./public/* $out/
          '';
        };
      };
    };
}
