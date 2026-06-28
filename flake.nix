{
  inputs = {
    flake-parts.url = "github:hercules-ci/flake-parts";
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";

    picocss = {
      url = "github:picocss/pico/v2.1.1";
      flake = false;
    };

    # Sveltia CMS prebuilt bundle, fetched from npm instead of loaded from a CDN.
    sveltia-cms = {
      url = "https://registry.npmjs.org/@sveltia/cms/-/cms-0.167.3.tgz";
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

        # Copy the nix-provided Sveltia CMS bundle into static/admin/ so Hugo
        # serves it locally instead of loading it from a CDN at runtime.
        sveltiaCmd = "cp ${inputs.sveltia-cms}/dist/sveltia-cms.js static/admin/sveltia-cms.js";
      in {
        devShells.default = pkgs.mkShell {
          packages = tools ++ [
            (pkgs.writeShellScriptBin "develop" ''
              ${sassCmd} --watch &
              ${sveltiaCmd}
              hugo server --buildDrafts --navigateToChanged "$@"
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
            ${sveltiaCmd}
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