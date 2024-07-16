export default [
    {
        context: ['/api'],
        target: 'http://localhost:5099',
        secure: false,
    },
    {
        context: ['/signalr'],
        target: 'http://localhost:5099',
        secure: false,
        ws: true,
    },
];
