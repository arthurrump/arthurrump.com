{
  inputs = {
    flake-parts.url = "github:hercules-ci/flake-parts";
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";

    pelican-plugins = {
      url = "github:getpelican/pelican-plugins";
      flake = false;
    };
  };

  outputs = inputs@{ self, flake-parts, ... }:
    flake-parts.lib.mkFlake { inherit inputs; } {
      systems = [ "x86_64-linux" "aarch64-linux" "aarch64-darwin" "x86_64-darwin" ];
      perSystem = { config, self', inputs', pkgs, system, ... }: 
      let 
        tools = with pkgs; with python312Packages; [
          pandoc
          haskellPackages.pandoc-crossref
          pelican
          markdown
          typogrify
          beautifulsoup4
          ruamel-yaml

          nodejs
          pnpm
          dart-sass
        ];

        PELICAN_PLUGINS = "${inputs.pelican-plugins}";
      in {
        devShells.default = pkgs.mkShell {
          packages = tools;
          inherit PELICAN_PLUGINS;
        };
      };
    };
}
