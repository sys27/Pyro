// @ts-check
const eslint = require('@eslint/js');
const tseslint = require('typescript-eslint');
const angular = require('angular-eslint');
const ngrx = require('@ngrx/eslint-plugin/v9');

module.exports = tseslint.config(
    {
        files: ['**/*.ts'],
        extends: [
            eslint.configs.recommended,
            ...tseslint.configs.recommended,
            ...tseslint.configs.stylistic,
            ...angular.configs.tsRecommended,
        ],
        processor: angular.processInlineTemplates,
        rules: {
            'prefer-const': 'off',
            '@typescript-eslint/no-inferrable-types': 'off',
            '@typescript-eslint/no-unused-vars': [
                'error',
                {
                    argsIgnorePattern: '^_',
                    caughtErrorsIgnorePattern: '^_',
                    destructuredArrayIgnorePattern: '^_',
                    varsIgnorePattern: '^_',
                },
            ],
            '@angular-eslint/directive-selector': [
                'error',
                {
                    type: 'attribute',
                    style: 'camelCase',
                },
            ],
            '@angular-eslint/component-selector': [
                'error',
                {
                    type: 'element',
                    style: 'kebab-case',
                },
            ],
        },
    },
    {
        files: ['**/*.ts'],
        extends: [...ngrx.configs.all],
        rules: {
            '@ngrx/no-typed-global-store': 'off',
        },
    },
    {
        files: ['**/*.html'],
        extends: [...angular.configs.templateRecommended, ...angular.configs.templateAccessibility],
        rules: {
            '@angular-eslint/template/no-autofocus': 'off',
            '@angular-eslint/template/elements-content': 'off',
        },
    },
);
